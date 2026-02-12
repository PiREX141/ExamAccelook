using System;
using System.Collections.Generic;
using System.Text;
using ExamAccelook.Contracts.ResponseModels;
using MediatR;

namespace ExamAccelook.Contracts.RequestModels
{
    public class GetAvailableTicketRequest : IRequest<List<GetAvailableTicketResponse>>
    {
        public string CategoryName { get; set; } = string.Empty;
        public string TicketCode { get; set; } = string.Empty;
        public string TicketName { get; set; } = string.Empty;
        public DateTime? EventDateFrom { get; set; }
        public DateTime? EventDateTo { get; set; }
        public decimal? MaxPrice { get; set; }

        public string OrderBy { get; set; } = "TicketCode";
        public string OrderDirection { get; set; } = "ASC";
    }
}
