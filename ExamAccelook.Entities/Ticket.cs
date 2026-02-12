using System;
using System.Collections.Generic;

namespace ExamAccelook.Entities;

public partial class Ticket
{
    public string TicketCode { get; set; } = null!;

    public int CategoryId { get; set; }

    public int TicketCreationOrder { get; set; }

    public string TicketName { get; set; } = null!;

    public int SequenceNumber { get; set; }

    public decimal Price { get; set; }

    public DateTime EventDate { get; set; }

    public int Quota { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    public virtual Category Category { get; set; } = null!;
}
