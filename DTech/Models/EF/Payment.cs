using System;
using System.Collections.Generic;

namespace DTech.Models.EF;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int PaymentMethodId { get; set; }

    public int? Status { get; set; }

    public DateOnly? Date { get; set; }

    public decimal? Amount { get; set; }

    public virtual Order? Order { get; set; }

    public virtual PaymentMethod PaymentMethod { get; set; } = null!;
}
