using System;
using System.Collections.Generic;
using System.Text;

namespace ExamAccelook.Contracts.ResponseModels
{
    public class GetBookedTicketResponse
    {
        public int QtyPerCategory { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public List<BookedTicketItem> Tickets { get; set; } = new List<BookedTicketItem>();
    }

    public class BookedTicketItem
    {
        public string TicketCode { get; set; } = string.Empty;
        public string TicketName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
    }
}
