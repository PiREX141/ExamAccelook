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
    public class GetBookedTicketHandler : IRequestHandler<GetBookedTicketRequest, List<GetBookedTicketResponse>>
    {
        private readonly ExamAccelookContext _db;
        private readonly ILogger<GetBookedTicketHandler> _logger;

        public GetBookedTicketHandler(ExamAccelookContext db, ILogger<GetBookedTicketHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<List<GetBookedTicketResponse>> Handle(GetBookedTicketRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetBookedTicket: BookedTicketId={BookedTicketId}", request.BookedTicketId);

            var query = await _db.BookingDetails
                .Where(d => d.BookedTicketId == request.BookedTicketId)
                .Include(d => d.TicketCodeNavigation)
                .ThenInclude(t => t.Category)
                .ToListAsync(cancellationToken);

            var groupedQuery = query
                .GroupBy(d => d.TicketCodeNavigation.Category.CategoryName)
                .Select(g => new GetBookedTicketResponse
                {
                    CategoryName = g.Key,
                    QtyPerCategory = g.Sum(x => x.Quantity),
                    Tickets = g.Select(x => new BookedTicketItem
                    {
                        TicketCode = x.TicketCode,
                        TicketName = x.TicketCodeNavigation.TicketName,
                        EventDate = x.TicketCodeNavigation.EventDate
                    }).ToList()
                })
                .ToList();

            _logger.LogInformation("GetBookedTicket returned {Groups} groups for: BookedTicketId={BookedTicketId}", groupedQuery.Count, request.BookedTicketId);

            return groupedQuery;
        }
    }
}
