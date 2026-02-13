using System;
using System.Collections.Generic;
using System.Text;
using ExamAccelook.Contracts.RequestModels;
using FluentValidation;

namespace ExamAccelook.Logics.Validators
{
    public class GetAvailableTicketValidator : AbstractValidator<GetAvailableTicketRequest>
    {
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

        public GetAvailableTicketValidator()
        {
            RuleFor(Q => Q.CategoryName)
                .MaximumLength(100)
                .WithMessage("Category Name cannot exceed 100 characters.");

            RuleFor(Q => Q.TicketCode)
                .MaximumLength(50)
                .WithMessage("Ticket Code cannot exceed 50 characters.");

            RuleFor(Q => Q.TicketName)
                .MaximumLength(200)
                .WithMessage("Ticket Name cannot exceed 200 characters.");

            RuleFor(Q => Q.MaxPrice)
                .GreaterThan(0)
                .When(Q => Q.MaxPrice.HasValue)
                .WithMessage("Max Price must be greater than 0.");

            RuleFor(Q => Q.EventDateFrom)
                .LessThanOrEqualTo(Q => Q.EventDateTo)
                .When(Q => Q.EventDateFrom.HasValue && Q.EventDateTo.HasValue)
                .WithMessage("Event Date From must be less than or equal to Event Date To.");

            RuleFor(Q => Q.EventDateTo)
                .GreaterThanOrEqualTo(Q => Q.EventDateFrom)
                .When(Q => Q.EventDateFrom.HasValue && Q.EventDateTo.HasValue)
                .WithMessage("Event Date To must be greater than or equal to Event Date From.");

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
