using System;
using System.Collections.Generic;

namespace DTech.Models.EF;

public partial class PaymentMethod
{
    public int PaymentMethodId { get; set; }

    public string? Description { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
