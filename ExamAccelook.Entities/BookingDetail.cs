using System;
using System.Collections.Generic;

namespace ExamAccelook.Entities;

public partial class BookingDetail
{
    public int BookingDetailId { get; set; }

    public int BookedTicketId { get; set; }

    public string TicketCode { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Subtotal { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual BookedTicket BookedTicket { get; set; } = null!;

    public virtual Ticket TicketCodeNavigation { get; set; } = null!;
}
