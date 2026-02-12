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
    public class RevokeTicketValidator : AbstractValidator<RevokeTicketRequest>
    {
        private readonly ExamAccelookContext _db;

        public RevokeTicketValidator(ExamAccelookContext db)
        {
            _db = db;

            RuleFor(r => r.BookedTicketId)
                .MustAsync(ExistBookedTicket)
                .WithMessage("BookedTicketId not found");

            RuleFor(r => r.TicketCode)
                .NotEmpty()
                .WithMessage("TicketCode is required")
                .MustAsync((req, code, ct) => BelongsToBookedTicket(req.BookedTicketId, code, ct))
                .WithMessage("TicketCode not found in booked ticket");

            RuleFor(r => r.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be at least 1")
                .MustAsync((req, quantity, ct) => QuantityWithinBooked(req.BookedTicketId, req.TicketCode, quantity, ct))
                .WithMessage("Quantity exceeds booked quantity");
        }

        private async Task<bool> ExistBookedTicket(int id, CancellationToken cancellationToken)
        {
            return await _db.BookedTickets.AnyAsync(b => b.BookedTicketId == id, cancellationToken);
        }

        private async Task<bool> BelongsToBookedTicket(int bookedTicketId, string ticketCode, CancellationToken cancellationToken)
        {
            return await _db.BookingDetails.AnyAsync(d => d.BookedTicketId == bookedTicketId && d.TicketCode == ticketCode, cancellationToken);
        }

        private async Task<bool> QuantityWithinBooked(int bookedTicketId, string ticketCode, int quantity, CancellationToken cancellationToken)
        {
            var detail = await _db.BookingDetails.FirstOrDefaultAsync(d => d.BookedTicketId == bookedTicketId && d.TicketCode == ticketCode, cancellationToken);
            if (detail == null)
            {
                return false; 
            }

            return quantity <= detail.Quantity;
        }
    }
}
