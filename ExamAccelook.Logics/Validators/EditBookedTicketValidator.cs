using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ExamAccelook.Contracts.RequestModels;
using ExamAccelook.Entities;
using FluentValidation;

namespace ExamAccelook.Logics.Validators
{
    public class EditBookedTicketValidator : AbstractValidator<EditBookedTicketRequest>
    {
        private readonly ExamAccelookContext _db;

        public EditBookedTicketValidator(ExamAccelookContext db)
        {
            _db = db;

            RuleFor(r => r.BookedTicketId)
                .MustAsync(ExistBookedTicket)
                .WithMessage("BookedTicketId not found");

            RuleFor(r => r.Tickets)
                .NotEmpty()
                .WithMessage("Ticket list cannot be empty");

            RuleFor(r => r).CustomAsync(async (request, context, cancellation) =>
            {
                if (request.Tickets == null) return;

                for (var i = 0; i < request.Tickets.Count; i++)
                {
                    var item = request.Tickets[i];

                    if (string.IsNullOrWhiteSpace(item.TicketCode))
                    {
                        context.AddFailure($"Tickets[{i}].TicketCode", "TicketCode is required");
                        continue;
                    }

                    if (!await BelongsToBookedTicket(request.BookedTicketId, item.TicketCode, cancellation))
                    {
                        context.AddFailure($"Tickets[{i}].TicketCode", "TicketCode not found in booked ticket");
                    }

                    if (item.NewQuantity < 1)
                    {
                        context.AddFailure($"Tickets[{i}].NewQuantity", "Quantity must be at least 1");
                    }
                    else if (!await QuantityWithinQuota(request.BookedTicketId, item, item.NewQuantity, cancellation))
                    {
                        context.AddFailure($"Tickets[{i}].NewQuantity", "Quantity exceeds available quota");
                    }
                }
            });
        }

        private async Task<bool> ExistBookedTicket(int id, CancellationToken cancellationToken)
        {
            return await _db.BookedTickets.AnyAsync(b => b.BookedTicketId == id, cancellationToken);
        }

        private async Task<bool> BelongsToBookedTicket(int bookedTicketId, string ticketCode, CancellationToken cancellationToken)
        {
            return await _db.BookingDetails.AnyAsync(d => d.BookedTicketId == bookedTicketId && d.TicketCode == ticketCode, cancellationToken);
        }

        private async Task<bool> QuantityWithinQuota(int bookedTicketId, EditBookedTicketItemRequest? item, int newQty, CancellationToken cancellationToken)
        {
            if (item == null)
            {
                return false;
            }

            var ticket = await _db.Tickets.FirstOrDefaultAsync(t => t.TicketCode == item.TicketCode, cancellationToken);
            if (ticket == null)
            {
                return false;
            }

            var detail = await _db.BookingDetails.FirstOrDefaultAsync(d => d.BookedTicketId == bookedTicketId && d.TicketCode == item.TicketCode, cancellationToken);

            var currentQty = detail?.Quantity ?? 0;

            var available = ticket.Quota + currentQty;

            return newQty <= available;

        }
    }
}
