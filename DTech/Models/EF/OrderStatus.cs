using System;
using System.Collections.Generic;

namespace DTech.Models.EF;

public partial class OrderStatus
{
    public int StatusId { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
