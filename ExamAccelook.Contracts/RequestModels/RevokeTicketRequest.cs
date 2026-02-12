using System;
using System.Collections.Generic;
using System.Text;
using ExamAccelook.Contracts.ResponseModels;
using MediatR;

namespace ExamAccelook.Contracts.RequestModels
{
    public class RevokeTicketRequest : IRequest<RevokeTicketResponse>
    {
        public int BookedTicketId { get; set; }
        public string TicketCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
