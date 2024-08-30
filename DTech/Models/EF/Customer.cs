using System;
using System.Collections.Generic;

namespace DTech.Models.EF;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Gender { get; set; }

    public DateOnly? DayOfBirth { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Account { get; set; }

    public string? Password { get; set; }

    public string? Image { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<CustomerCoupon> CustomerCoupons { get; set; } = new List<CustomerCoupon>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
