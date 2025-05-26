using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace DTech.Models.EF;

public partial class Order
{
    [Key]
    [Display(Name = "ID")]
    public int OrderId { get; set; }

    [Display(Name = "Customer")]
    public string? CustomerId { get; set; }

    [Display(Name = "Payment")]
    public int? PaymentId { get; set; }

    public int? ShippingId { get; set; }

    [Display(Name = "Status")]
    public int? StatusId { get; set; }

    [Display(Name = "Order Date")]
    public DateOnly? OrderDate { get; set; }

    //Billing 
    [Display(Name = "Billing Name")]
    public string? Name { get; set; }

    [Display(Name = "Billing Phone Number")]
    public string? Phone { get; set; }

    public string? Email { get; set; }

    [Display(Name = "Province")]
    public int? ProvinceId { get; set; }

    [Display(Name = "District")]
    public int? DistrictId { get; set; }

    [Display(Name = "Ward")]
    public int? WardId { get; set; }

    public string? Address { get; set; }

    //Shipping
    [Display(Name = "Consignee Name")]
    public string? NameReceive { get; set; }

    [Display(Name = "Consignee's Phone Number")]
    public string? PhoneReceive { get; set; }

    [Display(Name = "Shipping Province")]
    public int? ShippingProvinceId { get; set; }

    [Display(Name = "Shipping District")]
    public int? ShippingDistrictId { get; set; }

    [Display(Name = "Shipping Ward")]
    public int? ShippingWardId { get; set; }

    [Display(Name = "Shipping Address")]
    public string? ShippingAddress { get; set; }

    //Cost
    [Display(Name = "Total Cost")]
    public decimal? TotalCost { get; set; }

    [Display(Name = "Discount Cost")]
    public decimal? CostDiscount { get; set; }

    [Display(Name = "Shipping Cost")]
    public decimal? ShippingCost { get; set; }

    [Display(Name = "Final Cost")]
    public decimal? FinalCost { get; set; }

    public string? Note { get; set; }

    public virtual ApplicationUser? Customer { get; set; }

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    public virtual Payment? Payment { get; set; }

    public virtual Shipping? Shipping { get; set; }

    public virtual OrderStatus? Status { get; set; }

    public Province? Province { get; set; }
    public District? District { get; set; }
    public Ward? Ward { get; set; }
    public Province? ShippingProvince { get; set; }
    public District? ShippingDistrict { get; set; }
    public Ward? ShippingWard { get; set; }
}
