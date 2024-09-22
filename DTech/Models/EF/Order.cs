using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DTech.Models.EF;

public partial class Order
{
    [Key]
    public int OrderId { get; set; }

    [Display(Name = "Customer")]
    public int? CustomerId { get; set; }

    [Display(Name = "Payment")]
    public int? PaymentId { get; set; }

    public int? ShippingId { get; set; }

    [Display(Name = "Status")]
    public int? StatusId { get; set; }

    [Display(Name = "Order Date")]
    public DateOnly? OrderDate { get; set; }

    public string? Address { get; set; }

    [Display(Name = "Consignee Name")]
    public string? NameReceive { get; set; }

    [Display(Name = "Consignee's Phone Number")]
    public string? PhoneReceive { get; set; }

    [Display(Name = "Total Cost")]
    public decimal? TotalCost { get; set; }

    [Display(Name = "Discount Cost")]
    public decimal? CostDiscount { get; set; }

    [Display(Name = "Shipping Cost")]
    public decimal? ShippingCost { get; set; }

    [Display(Name = "Final Cost")]
    public decimal? FinalCost { get; set; }

    public string? Note { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Payment? Payment { get; set; }

    public virtual Shipping? Shipping { get; set; }

    public virtual OrderStatus? Status { get; set; }
}
