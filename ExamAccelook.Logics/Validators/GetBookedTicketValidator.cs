using System;
using System.Collections.Generic;
using System.Text;
using ExamAccelook.Contracts.RequestModels;
using ExamAccelook.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ExamAccelook.Logics.Validators
{
    public class GetBookedTicketValidator : AbstractValidator<GetBookedTicketRequest>
    {
        private readonly ExamAccelookContext _db;

        public GetBookedTicketValidator(ExamAccelookContext db)
        {
            _db = db;

            RuleFor(x => x.BookedTicketId)
                .GreaterThan(0).WithMessage("BookedTicketId must be greater than 0.")
                .MustAsync(ExistInDatabase).WithMessage("BookedTicketId not found.");
        }

        private async Task<bool> ExistInDatabase(int bookedTicketId, CancellationToken cancellationToken)
        {
            return await _db.BookedTickets.AnyAsync(b => b.BookedTicketId == bookedTicketId, cancellationToken);
        }
    }
}
