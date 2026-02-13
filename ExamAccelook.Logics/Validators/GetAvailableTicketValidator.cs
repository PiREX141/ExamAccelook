using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ExamAccelook.Entities;
using ExamAccelook.Contracts.RequestModels;
using FluentValidation;

namespace ExamAccelook.Logics.Validators
{
    public class GetAvailableTicketValidator : AbstractValidator<GetAvailableTicketRequest>
    {
        private readonly ExamAccelookContext _db;

        private static readonly string[] OrderByColumns = new[]
        {
            "CategoryName",
            "TicketCode",
            "TicketName",
            "EventDate",
            "Price",
            "Quota"
        };

        private static readonly string[] OrderDirections = new[] { "ASC", "DESC" };

        public GetAvailableTicketValidator(ExamAccelookContext db)
        {
            _db = db;
            RuleFor(Q => Q.CategoryName)
                .MaximumLength(100)
                .WithMessage("Category Name cannot exceed 100 characters.");

            RuleFor(Q => Q.CategoryName)
                .MustAsync(async (categoryName, cancellation) =>
                {
                    if (string.IsNullOrWhiteSpace(categoryName)) return true;
                    return await _db.Categories.AnyAsync(c => c.CategoryName.Contains(categoryName), cancellation);
                })
                .WithMessage("Category not found");

            RuleFor(Q => Q.TicketCode)
                .MaximumLength(50)
                .WithMessage("Ticket Code cannot exceed 50 characters.");

            RuleFor(Q => Q.TicketCode)
                .MustAsync(async (ticketCode, cancellation) =>
                {
                    if (string.IsNullOrWhiteSpace(ticketCode)) return true;
                    return await _db.Tickets.AnyAsync(t => t.TicketCode.Contains(ticketCode), cancellation);
                })
                .WithMessage("Ticket Code not found");

            RuleFor(Q => Q.TicketName)
                .MaximumLength(200)
                .WithMessage("Ticket Name cannot exceed 200 characters.");

            RuleFor(Q => Q.TicketName)
                .MustAsync(async (ticketName, cancellation) =>
                {
                    if (string.IsNullOrWhiteSpace(ticketName)) return true;
                    return await _db.Tickets.AnyAsync(t => t.TicketName.Contains(ticketName), cancellation);
                })
                .WithMessage("Ticket Name not found");

            RuleFor(Q => Q.MaxPrice)
                .GreaterThan(0)
                .When(Q => Q.MaxPrice.HasValue)
                .WithMessage("Max Price must be greater than 0.");

            RuleFor(Q => Q.MaxPrice)
                .MustAsync(async (maxPrice, cancellation) =>
                {
                    if (!maxPrice.HasValue) return true;
                    return await _db.Tickets.AnyAsync(t => t.Price <= maxPrice.Value, cancellation);
                })
                .When(Q => Q.MaxPrice.HasValue)
                .WithMessage("No tickets found with price less than or equal to Max Price");

            RuleFor(Q => Q.EventDateFrom)
                .LessThanOrEqualTo(Q => Q.EventDateTo)
                .When(Q => Q.EventDateFrom.HasValue && Q.EventDateTo.HasValue)
                .WithMessage("Event Date From must be less than or equal to Event Date To.");

            RuleFor(Q => Q.EventDateTo)
                .GreaterThanOrEqualTo(Q => Q.EventDateFrom)
                .When(Q => Q.EventDateFrom.HasValue && Q.EventDateTo.HasValue)
                .WithMessage("Event Date To must be greater than or equal to Event Date From.");

            RuleFor(Q => Q).CustomAsync(async (request, context, cancellation) =>
            {
                if (!request.EventDateFrom.HasValue && !request.EventDateTo.HasValue) return;

                var q = _db.Tickets.AsQueryable();

                if (request.EventDateFrom.HasValue)
                {
                    q = q.Where(t => t.EventDate >= request.EventDateFrom.Value);
                }

                if (request.EventDateTo.HasValue)
                {
                    q = q.Where(t => t.EventDate <= request.EventDateTo.Value);
                }

                var exists = await q.AnyAsync(cancellation);
                if (!exists)
                {
                    context.AddFailure("EventDateFrom", "No tickets found in the specified event date range.");
                }
            });

            RuleFor(Q => Q.OrderBy)
                .NotEmpty()
                .WithMessage("Order By column cannot be empty.")
                .Must(BeValidOrderByColumn)
                .WithMessage($"Order By must be one of: {string.Join(", ", OrderByColumns)}.");

            RuleFor(Q => Q.OrderDirection)
                .NotEmpty()
                .WithMessage("Order Direction cannot be empty.")
                .Must(BeValidOrderDirection)
                .WithMessage($"Order Direction must be one of: {string.Join(", ", OrderDirections)}.");

            RuleFor(Q => Q.PageNumber)
                .GreaterThan(0)
                .WithMessage("PageNumber must be greater than 0.");

            RuleFor(Q => Q.PageSize)
                .GreaterThan(0)
                .LessThanOrEqualTo(100)
                .WithMessage("PageSize must be greater than 0 and less than or equal to 100.");
        }

        private bool BeValidOrderByColumn(string orderBy)
        {
            if (string.IsNullOrWhiteSpace(orderBy))
            {
                return false;
            }

            return Array.Exists(OrderByColumns, col =>
                col.Equals(orderBy, StringComparison.OrdinalIgnoreCase));
        }

        private bool BeValidOrderDirection(string orderDirection)
        {
            if (string.IsNullOrWhiteSpace(orderDirection))
            {
                return false;
            }

            return Array.Exists(OrderDirections, dir =>
                dir.Equals(orderDirection, StringComparison.OrdinalIgnoreCase));
        }
    }
}
