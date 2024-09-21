using DTech.Library.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTech.Models.EF;

public partial class Product
{
    [Key]
    [Display(Name = "ID")]
    public int ProductId { get; set; }

    public int? BrandId { get; set; }

    public int? SupplierId { get; set; }

    public int? CategoryId { get; set; }

    [Required(ErrorMessage = "Please enter name of product")]
    public string? Name { get; set; }

    public string? Slug { get; set; }

    [Required(ErrorMessage = "Please enter warranty of product")]
    public string? Warranty { get; set; }

    [Display(Name = "Product Status")]
    public bool? StatusProduct { get; set; }

    [Required(ErrorMessage = "Please enter price of product")]
    public decimal? Price { get; set; }

    public decimal? Discount { get; set; }

    [Display(Name = "Discount End Date")]
    public DateOnly? EndDateDiscount { get; set; }

    public int? Views { get; set; }

    [Display(Name = "Date of Manufacture")]
    public DateOnly? DateOfManufacture { get; set; }

    [Display(Name = "Made In")]
    public string? MadeIn { get; set; }

    [Display(Name = "Promotion Gift")]
    public string? PromotionalGift { get; set; }

    public string? Photo { get; set; }

    public string? Description { get; set; }

    [Display(Name = "Update Date")]
    public DateTime? UpdateDate { get; set; }

    [Display(Name = "Created By")]
    public string? CreatedBy { get; set; }

    [Display(Name = "Create Date")]
    public DateTime? CreateDate { get; set; }

    [Display(Name = "Updated By")]
    public string? UpdatedBy { get; set; }

    public int? Status { get; set; }

    public virtual Brand? Brand { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<ProductComment> ProductComments { get; set; } = new List<ProductComment>();

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ICollection<Specification> Specifications { get; set; } = new List<Specification>();

    public virtual Supplier? Supplier { get; set; }

    [NotMapped]
    [FileExtension]
    public IFormFile? PhotoUpload { get; set; }
}
