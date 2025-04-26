using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DTech.Models.EF;

public partial class Coupon
{
    [Key]
    public int CouponId { get; set; }

    [Required(ErrorMessage = "Please enter name of coupon")]
    public string? Name { get; set; }

    public string? Slug { get; set; }

    [Required(ErrorMessage = "Please enter code")]
    public string? Code { get; set; }

    [Required(ErrorMessage = "Please enter discount")]
    public decimal? Discount { get; set; }

    [Required(ErrorMessage = "Please enter condition")]
    public int? Condition { get; set; }

    [Required(ErrorMessage = "Please enter detail")]
    public string? Detail { get; set; }

    public DateOnly? EndDate { get; set; }

    public int? Status { get; set; }

    [Display(Name = "Created By")]
    public string? CreatedBy { get; set; }

    [Display(Name = "Create Date")]
    public DateTime? CreateDate { get; set; }

    [Display(Name = "Updated By")]
    public string? UpdatedBy { get; set; }

    [Display(Name = "Update Date")]
    public DateTime? UpdateDate { get; set; }

    public virtual ICollection<CustomerCoupon> CustomerCoupons { get; set; } = new List<CustomerCoupon>();
}
