using DTech.Library.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTech.Models.EF;

public partial class Brand
{
    [Key]
    public int BrandId { get; set; }

    [Required(ErrorMessage = "Please enter name of brand")]
    public string? Name { get; set; }

    public string? Slug { get; set; }

    public string? Logo { get; set; }

    [Display(Name = "Created By")]
    public string? CreatedBy { get; set; }

    [Display(Name = "Create Date")]
    public DateTime? CreateDate { get; set; }

    [Display(Name = "Updated By")]
    public string? UpdatedBy { get; set; }

    [Display(Name = "Update Date")]
    public DateTime? UpdateDate { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    [NotMapped]
    [FileExtension]
    public IFormFile? LogoUpload { get; set; }
}
