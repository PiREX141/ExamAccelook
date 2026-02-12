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
    public class PostBookTicketHandler : IRequestHandler<PostBookTicketRequest, PostBookTicketResponse>
    {
        private readonly ExamAccelookContext _db;
        private readonly ILogger<PostBookTicketHandler> _logger;

        public PostBookTicketHandler(ExamAccelookContext db, ILogger<PostBookTicketHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<PostBookTicketResponse> Handle(PostBookTicketRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("PostBookTicket: TicketsCount={TicketsCount}", request.Tickets?.Count ?? 0);

            var ticketCodes = request.Tickets.Select(t => t.TicketCode).ToList();

            var tickets = await _db.Tickets
                .Include(t => t.Category)
                .Where(t => ticketCodes.Contains(t.TicketCode))
                .ToListAsync(cancellationToken);

            var booking = new BookedTicket();
            booking.BookingDate = DateTime.UtcNow;
            booking.CreatedAt = DateTime.UtcNow;

            _db.BookedTickets.Add(booking);

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Booking created for: BookedTicketId={BookedTicketId}", booking.BookedTicketId);

            decimal totalAmount = 0;

            var details = new List<BookingDetail>();

            foreach (var item in request.Tickets)
            {
                var ticket = tickets.FirstOrDefault(t => t.TicketCode == item.TicketCode);
                if (ticket == null)
                {
                    continue;
                }

                var unitPrice = ticket.Price;
                var subtotal = unitPrice * item.Quantity;

                var detail = new BookingDetail
                {
                    BookedTicketId = booking.BookedTicketId,
                    TicketCode = ticket.TicketCode,
                    Quantity = item.Quantity,
                    UnitPrice = unitPrice,
                    Subtotal = subtotal,
                    CreatedAt = DateTime.UtcNow
                };

                details.Add(detail);

                ticket.Quota -= item.Quantity;

                totalAmount += subtotal;
            }

            _db.BookingDetails.AddRange(details);

            booking.TotalAmount = totalAmount;

            await _db.SaveChangesAsync(cancellationToken);

            var grouped = details
                .Join(tickets, d => d.TicketCode, t => t.TicketCode, (d, t) => new { Detail = d, Ticket = t })
                .GroupBy(x => x.Ticket.Category.CategoryName)
                .Select(g => new TicketsPerCategory
                {
                    CategoryName = g.Key,
                    SummaryPrice = g.Sum(x => x.Detail.Subtotal),
                    Tickets = g.Select(x => new PostBookTicketItem
                    {
                        TicketCode = x.Ticket.TicketCode,
                        TicketName = x.Ticket.TicketName,
                        Price = x.Detail.Subtotal
                    }).ToList()
                }).ToList();

            var response = new PostBookTicketResponse
            {
                PriceSummary = totalAmount,
                TicketsPerCategories = grouped
            };

            return response;
        }
    }
}
