using System;
using System.Collections.Generic;
using System.Text;

namespace ExamAccelook.Contracts.ResponseModels
{
    public class GetAvailableTicketPagedResponse
    {
        public List<GetAvailableTicketResponse> Tickets { get; set; } = new List<GetAvailableTicketResponse>();
        public int TotalTickets { get; set; }
    }
}
