using System;
using System.Collections.Generic;
using System.Text;
using ExamAccelook.Contracts.ResponseModels;
using MediatR;

namespace ExamAccelook.Contracts.RequestModels
{
    public class GetBookedTicketRequest : IRequest<List<GetBookedTicketResponse>>
    {
        public int BookedTicketId { get; set; }
    }
}
