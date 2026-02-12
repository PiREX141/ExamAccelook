using System;
using System.Collections.Generic;
using System.Text;

namespace ExamAccelook.Contracts.ResponseModels
{
    public class PostBookTicketResponse
    {
        public decimal PriceSummary { get; set; }
        public List<TicketsPerCategory> TicketsPerCategories { get; set; } = new();
    }

    public class TicketsPerCategory
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal SummaryPrice { get; set; }
        public List<PostBookTicketItem> Tickets { get; set; } = new();
    }

    public class PostBookTicketItem
    {
        public string TicketCode { get; set; } = string.Empty;
        public string TicketName { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
