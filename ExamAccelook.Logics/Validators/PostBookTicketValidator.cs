using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ExamAccelook.Contracts.RequestModels;
using ExamAccelook.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ExamAccelook.Logics.Validators
{
    public class PostBookTicketValidator : AbstractValidator<PostBookTicketRequest>
    {
        private readonly ExamAccelookContext _db;

        public PostBookTicketValidator(ExamAccelookContext db)
        {
            _db = db;

            RuleFor(Q => Q.Tickets)
                .NotEmpty().WithMessage("Ticket list cannot be empty");

            RuleForEach(Q => Q.Tickets).ChildRules(ticket =>
            {
                ticket.RuleFor(t => t.TicketCode)
                    .NotEmpty().WithMessage("TicketCode is required")
                    .MustAsync(ExistInDatabase).WithMessage("TicketCode not found")
                    .MustAsync(NotPastEvent).WithMessage("Ticket cannot be booked after event date");

                ticket.RuleFor(t => t.Quantity)
                    .GreaterThan(0).WithMessage("Quantity must be greater than 0")
                    .Must((item, qty) =>
                    {
                        try
                        {
                            var ticket = _db.Tickets.FirstOrDefault(t => t.TicketCode == item.TicketCode);
                            if (ticket == null) return false;
                            if (ticket.Quota <= 0) return false;
                            return qty <= ticket.Quota;
                        }
                        catch
                        {
                            return false;
                        }
                    }).WithMessage("Quantity exceeds available quota");
            });
        }

        private async Task<bool> ExistInDatabase(string ticketCode, CancellationToken cancellationToken)
        {
            return await _db.Tickets.AnyAsync(t => t.TicketCode == ticketCode, cancellationToken);
        }

        private async Task<bool> NotPastEvent(string ticketCode, CancellationToken cancellationToken)
        {
            var ticket = await _db.Tickets.FirstOrDefaultAsync(t => t.TicketCode == ticketCode, cancellationToken);
            if (ticket == null)
            {
                return false;
            }

            return ticket.EventDate > DateTime.UtcNow;
        }
    }
}
