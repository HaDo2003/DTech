using System;
using System.Collections.Generic;

namespace DTech.Models.EF;

public partial class Order
{
    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    public int PaymentId { get; set; }

    public int ShippingId { get; set; }

    public int StatusId { get; set; }

    public DateOnly? OrderDate { get; set; }

    public string? Address { get; set; }

    public string? NameReceive { get; set; }

    public string? PhoneReceive { get; set; }

    public decimal? TotalCost { get; set; }

    public decimal? CostDiscount { get; set; }

    public decimal? ShippingCost { get; set; }

    public decimal? FinalCost { get; set; }

    public string? Note { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Payment OrderNavigation { get; set; } = null!;

    public virtual Shipping Shipping { get; set; } = null!;

    public virtual OrderStatus Status { get; set; } = null!;
}
