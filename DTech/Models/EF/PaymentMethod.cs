using System;
using System.Collections.Generic;

namespace DTech.Models.EF;

public partial class PaymentMethod
{
    public int PaymentMethodId { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
