using System;
using System.Collections.Generic;

namespace DTech.Models.EF;

public partial class OrderCoupon
{
    public int? OrderId { get; set; }

    public int? CouponId { get; set; }

    public virtual Coupon? Coupon { get; set; }

    public virtual Order? Order { get; set; }
}
