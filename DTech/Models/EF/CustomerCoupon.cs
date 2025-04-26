using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DTech.Models.EF;

public partial class CustomerCoupon
{
    [Key]
    public int Id { get; set; }

    public int? CustomerId { get; set; }

    public int? CouponId { get; set; }

    public int? Status { get; set; }

    public DateOnly? EndDate { get; set; }

    public virtual Coupon? Coupon { get; set; }

    public virtual ApplicationUser? Customer { get; set; }
}
