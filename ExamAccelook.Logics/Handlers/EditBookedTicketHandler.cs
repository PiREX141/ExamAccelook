using System;
using System.Collections.Generic;
using System.Text;
using ExamAccelook.Contracts.RequestModels;
using ExamAccelook.Contracts.ResponseModels;
using ExamAccelook.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace ExamAccelook.Logics.Handlers
{
    public class EditBookedTicketHandler : IRequestHandler<EditBookedTicketRequest, List<EditBookedTicketResponse>>
    {
        private readonly ExamAccelookContext _db;
        private readonly ILogger<EditBookedTicketHandler> _logger;

        public EditBookedTicketHandler(ExamAccelookContext db, ILogger<EditBookedTicketHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<List<EditBookedTicketResponse>> Handle(EditBookedTicketRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("EditBookedTicket: BookedTicketId={BookedTicketId} TicketsCount={TicketsCount}", request.BookedTicketId, request.Tickets?.Count ?? 0);

            // load relevant tickets and booking details
            var ticketCodes = request.Tickets.Select(t => t.TicketCode).ToList();

            var tickets = await _db.Tickets
                .Include(t => t.Category)
                .Where(t => ticketCodes.Contains(t.TicketCode))
                .ToListAsync(cancellationToken);

            var details = await _db.BookingDetails
                .Where(d => d.BookedTicketId == request.BookedTicketId && ticketCodes.Contains(d.TicketCode))
                .ToListAsync(cancellationToken);

            foreach (var item in request.Tickets)
            {
                var detail = details.FirstOrDefault(d => d.TicketCode == item.TicketCode);
                var ticket = tickets.FirstOrDefault(t => t.TicketCode == item.TicketCode);

                if (detail == null || ticket == null)
                {
                    continue;
                }

                var oldQuota = ticket.Quota;
                ticket.Quota = ticket.Quota + detail.Quantity - item.NewQuantity;

                detail.Quantity = item.NewQuantity;
                detail.UpdatedAt = System.DateTime.UtcNow;

                _logger.LogInformation("Adjusted {TicketCode}: OldQuota={OldQuota} NewQuota={NewQuota} BookedTicketId={BookedTicketId} NewQuantity={NewQuantity}", item.TicketCode, oldQuota, ticket.Quota, request.BookedTicketId, item.NewQuantity);
            }

            await _db.SaveChangesAsync(cancellationToken);

            var response = details
                .Join(tickets, d => d.TicketCode, t => t.TicketCode, (d, t) => new EditBookedTicketResponse
                {
                    TicketCode = d.TicketCode,
                    TicketName = t.TicketName,
                    Quantity = d.Quantity,
                    CategoryName = t.Category.CategoryName
                })
                .ToList();

            _logger.LogInformation("EditBookedTicket success for: BookedTicketId={BookedTicketId}", request.BookedTicketId);

            return response;
        }
    }
}
