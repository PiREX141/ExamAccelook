using System;
using System.Collections.Generic;

namespace ExamAccelook.Entities;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string CategoryPrefix { get; set; } = null!;

    public int LastSequenceNumber { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
