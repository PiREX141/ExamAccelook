using System;
using System.Collections.Generic;
using System.Text;

namespace ExamAccelook.Contracts.ResponseModels
{
    public class EditBookedTicketResponse
    {
        public string TicketCode { get; set; } = string.Empty;
        public string TicketName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
