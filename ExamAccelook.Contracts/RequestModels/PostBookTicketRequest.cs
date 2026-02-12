using System;
using System.Collections.Generic;
using System.Text;
using ExamAccelook.Contracts.ResponseModels;
using MediatR;

namespace ExamAccelook.Contracts.RequestModels
{
    public class PostBookTicketRequest : IRequest<PostBookTicketResponse>
    {
        public List<PostBookTicketItemRequest> Tickets { get; set; } = new();
    }

    public class PostBookTicketItemRequest
    {
        public string TicketCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
