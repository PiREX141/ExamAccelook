using System;
using System.Collections.Generic;

namespace ExamAccelook.Entities;

public partial class BookedTicket
{
    public int BookedTicketId { get; set; }

    public DateTime BookingDate { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();
}
