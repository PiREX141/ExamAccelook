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
    public class RevokeTicketHandler : IRequestHandler<RevokeTicketRequest, RevokeTicketResponse>
    {
        private readonly ExamAccelookContext _db;
        private readonly ILogger<RevokeTicketHandler> _logger;

        public RevokeTicketHandler(ExamAccelookContext db, ILogger<RevokeTicketHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<RevokeTicketResponse> Handle(RevokeTicketRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Revoking for: BookedTicketId={BookedTicketId} TicketCode={TicketCode} Quantity={Quantity}", request.BookedTicketId, 
                request.TicketCode, request.Quantity);

            var detail = await _db.BookingDetails
                .FirstOrDefaultAsync(d => d.BookedTicketId == request.BookedTicketId && d.TicketCode == request.TicketCode, cancellationToken);

            if (detail == null)
            {
                _logger.LogWarning("BookingDetail not found: BookedTicketId={BookedTicketId} TicketCode={TicketCode}", request.BookedTicketId, request.TicketCode);
                return null!;
            }

            var ticket = await _db.Tickets.Include(t => t.Category).FirstOrDefaultAsync(t => t.TicketCode == request.TicketCode, cancellationToken);

            if (ticket == null)
            {
                _logger.LogWarning("TicketCode not found: TicketCode={TicketCode}", request.TicketCode);
                return null!;
            }

            detail.Quantity -= request.Quantity;
            if (detail.Quantity < 0)
            {
                detail.Quantity = 0;
            } 
            detail.UpdatedAt = System.DateTime.UtcNow;

            ticket.Quota += request.Quantity;

            _logger.LogInformation("Revoked {Quantity} from: BookedTicketId={BookedTicketId} TicketCode={TicketCode}. New detail.Quantity={NewQuantity} Ticket.Quota={NewQuota}", 
                request.Quantity, request.BookedTicketId, request.TicketCode, detail.Quantity, ticket.Quota);

            if (detail.Quantity == 0)
            {
                _db.BookingDetails.Remove(detail);
            }

            var remainingDetails = await _db.BookingDetails.Where(d => d.BookedTicketId == request.BookedTicketId).ToListAsync(cancellationToken);

            if (!remainingDetails.Any())
            {
                var booked = await _db.BookedTickets.FirstOrDefaultAsync(b => b.BookedTicketId == request.BookedTicketId, cancellationToken);
                if (booked != null)
                {
                    _db.BookedTickets.Remove(booked);
                }
            }

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Revoke Success for: BookedTicketId={BookedTicketId} TicketCode={TicketCode}", request.BookedTicketId, request.TicketCode);

            var remainingQty = detail.Quantity > 0 ? detail.Quantity : 0;

            var response = new RevokeTicketResponse
            {
                TicketCode = ticket.TicketCode,
                TicketName = ticket.TicketName,
                CategoryName = ticket.Category?.CategoryName ?? string.Empty,
                Quantity = remainingQty
            };

            return response;
        }
    }
}
