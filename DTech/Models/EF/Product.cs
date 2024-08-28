using System;
using System.Collections.Generic;

namespace DTech.Models.EF;

public partial class Product
{
    public int ProductId { get; set; }

    public int BrandId { get; set; }

    public int SupplierId { get; set; }

    public int CategoryId { get; set; }

    public string? Name { get; set; }

    public string? Slug { get; set; }

    public string? Warranty { get; set; }

    public bool? StatusProduct { get; set; }

    public decimal? Price { get; set; }

    public decimal? Discount { get; set; }

    public DateOnly? EndDateDiscount { get; set; }

    public int? Views { get; set; }

    public DateOnly? DateOfManufacture { get; set; }

    public string? MadeIn { get; set; }

    public string? PromotionalGift { get; set; }

    public string? Photo { get; set; }

    public string? Description { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? UpdatedBy { get; set; }

    public int? Status { get; set; }

    public virtual Brand Brand { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<ProductComment> ProductComments { get; set; } = new List<ProductComment>();

    public virtual ICollection<Specification> Specifications { get; set; } = new List<Specification>();

    public virtual Supplier Supplier { get; set; } = null!;
}
