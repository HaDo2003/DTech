using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DTech.Models.ViewModel;

namespace DTech.Models.EF;

public partial class EcommerceWebContext : IdentityDbContext<ApplicationUser>
{
    public EcommerceWebContext()
    {
    }

    public EcommerceWebContext(DbContextOptions<EcommerceWebContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Advertisement> Advertisements { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartProduct> CartProducts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Coupon> Coupons { get; set; }

    public virtual DbSet<CustomerAddress> CustomerAddresses { get; set; }

    public virtual DbSet<CustomerCoupon> CustomerCoupons { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Link> Links { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderCoupon> OrderCoupons { get; set; }

    public virtual DbSet<OrderProduct> OrderProducts { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<PostCategory> PostCategories { get; set; }

    public virtual DbSet<PostComment> PostComments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductComment> ProductComments { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<Shipping> Shippings { get; set; }

    public virtual DbSet<Specification> Specifications { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }


    protected void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdentityUserLogin<String>>().HasNoKey();
        modelBuilder.Entity<IdentityUserToken<String>>().HasNoKey();
        modelBuilder.Entity<IdentityUserRole<String>>().HasNoKey();
    }
}
