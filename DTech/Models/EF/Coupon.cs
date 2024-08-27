using System;
using System.Collections.Generic;

namespace DTech.Models.EF;

public partial class Coupon
{
    public int CouponId { get; set; }

    public string? Name { get; set; }

    public string? Slug { get; set; }

    public int? Code { get; set; }

    public decimal? Discount { get; set; }

    public int? Condition { get; set; }

    public string? Detail { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdateDate { get; set; }
}
