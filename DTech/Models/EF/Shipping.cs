using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DTech.Models.EF;

public partial class Shipping
{
    [Key]
    public int ShippingId { get; set; }

    public DateOnly? DelivaryDate { get; set; }

    public DateOnly? ReceivedDate { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
