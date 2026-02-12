using System;
using System.Collections.Generic;
using System.Text;
using ExamAccelook.Contracts.ResponseModels;
using MediatR;

namespace ExamAccelook.Contracts.RequestModels
{
    public class EditBookedTicketRequest : IRequest<List<EditBookedTicketResponse>>
    {
        public int BookedTicketId { get; set; }
        public List<EditBookedTicketItemRequest> Tickets { get; set; } = new();
    }

    public class EditBookedTicketItemRequest
    {
        public string TicketCode { get; set; } = string.Empty;
        public int NewQuantity { get; set; }
    }
}
