using System;
using System.Collections.Generic;

namespace DTech.Models.EF;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int? PaymentMethodId { get; set; }

    public int? Status { get; set; }

    public DateOnly? Date { get; set; }

    public decimal? Amount { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual PaymentMethod? PaymentMethod { get; set; }
}
