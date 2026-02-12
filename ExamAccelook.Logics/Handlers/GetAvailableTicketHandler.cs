using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExamAccelook.Contracts.RequestModels;
using ExamAccelook.Contracts.ResponseModels;
using ExamAccelook.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace ExamAccelook.Logics.Handlers
{
    public class GetAvailableTicketHandler : IRequestHandler<GetAvailableTicketRequest, GetAvailableTicketPagedResponse>
    {
        private readonly ExamAccelookContext _db;
        private readonly ILogger<GetAvailableTicketHandler> _logger;

        public GetAvailableTicketHandler(ExamAccelookContext db, ILogger<GetAvailableTicketHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<GetAvailableTicketPagedResponse> Handle(GetAvailableTicketRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GettingAvailableTicket: TicketCode={TicketCode}", request.TicketCode);

            var query = _db.Tickets
                .Join(_db.Categories,
                    ticket => ticket.CategoryId,
                    category => category.CategoryId,
                    (ticket, category) => new
                    {
                        Ticket = ticket,
                        Category = category
                    });

            query = query.Where(Q => Q.Ticket.Quota > 0);

            if (!string.IsNullOrWhiteSpace(request.CategoryName))
            {
                query = query.Where(Q => Q.Category.CategoryName.Contains(request.CategoryName));
            }

            if (!string.IsNullOrWhiteSpace(request.TicketCode))
            {
                query = query.Where(Q => Q.Ticket.TicketCode.Contains(request.TicketCode));
            }

            if (!string.IsNullOrWhiteSpace(request.TicketName))
            {
                query = query.Where(Q => Q.Ticket.TicketName.Contains(request.TicketName));
            }

            if (request.MaxPrice.HasValue)
            {
                query = query.Where(Q => Q.Ticket.Price <= request.MaxPrice.Value);
            }

            if (request.EventDateFrom.HasValue)
            {
                query = query.Where(Q => Q.Ticket.EventDate >= request.EventDateFrom.Value);
            }

            if (request.EventDateTo.HasValue)
            {
                query = query.Where(Q => Q.Ticket.EventDate <= request.EventDateTo.Value);
            }

            var projectedQuery = query.Select(Q => new GetAvailableTicketResponse
            {
                EventDate = Q.Ticket.EventDate,
                Quota = Q.Ticket.Quota,
                TicketCode = Q.Ticket.TicketCode,
                TicketName = Q.Ticket.TicketName,
                CategoryName = Q.Category.CategoryName,
                Price = Q.Ticket.Price
            });

            projectedQuery = ApplyOrdering(projectedQuery, request.OrderBy, request.OrderDirection);

            var totalCount = await projectedQuery.CountAsync(cancellationToken);

            var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
            var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

            var pagedQuery = projectedQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var items = await pagedQuery.ToListAsync(cancellationToken);

            _logger.LogInformation("GetAvailableTicket success for: {Count} items (page {PageNumber} size {PageSize}) out of {Total}", 
                items.Count, pageNumber, pageSize, totalCount);

            return new GetAvailableTicketPagedResponse
            {
                Tickets = items,
                TotalTickets = totalCount
            };
        }

        private IQueryable<GetAvailableTicketResponse> ApplyOrdering(
            IQueryable<GetAvailableTicketResponse> query,
            string orderBy,
            string orderDirection)
        {
            var isAscending = string.IsNullOrWhiteSpace(orderDirection) ||
                              orderDirection.Equals("ASC", StringComparison.OrdinalIgnoreCase);

            if (string.IsNullOrWhiteSpace(orderBy))
            {
                orderBy = "TicketCode";
            }

            return orderBy.ToUpperInvariant() switch
            {
                "CATEGORYNAME" => isAscending
                    ? query.OrderBy(Q => Q.CategoryName)
                    : query.OrderByDescending(Q => Q.CategoryName),

                "TICKETCODE" => isAscending
                    ? query.OrderBy(Q => Q.TicketCode)
                    : query.OrderByDescending(Q => Q.TicketCode),

                "TICKETNAME" => isAscending
                    ? query.OrderBy(Q => Q.TicketName)
                    : query.OrderByDescending(Q => Q.TicketName),

                "EVENTDATE" => isAscending
                    ? query.OrderBy(Q => Q.EventDate)
                    : query.OrderByDescending(Q => Q.EventDate),

                "PRICE" => isAscending
                    ? query.OrderBy(Q => Q.Price)
                    : query.OrderByDescending(Q => Q.Price),

                "QUOTA" => isAscending
                    ? query.OrderBy(Q => Q.Quota)
                    : query.OrderByDescending(Q => Q.Quota),

                _ => isAscending
                    ? query.OrderBy(Q => Q.TicketCode)
                    : query.OrderByDescending(Q => Q.TicketCode)
            };
        }
    }
}
