using DTech.Library.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DTech.Models.EF;

public partial class Advertisement
{
    [Key]
    public int AdvId { get; set; }

    [Required(ErrorMessage = "Please enter name")]
    [MaxLength(50, ErrorMessage = "Account can only have a maximum of 50 characters ")]
    public string? Name { get; set; }

    public string? Slug { get; set; }

    public string? Image { get; set; }

    [Display(Name = "Created By")]
    public string? CreatedBy { get; set; }

    [Display(Name = "Create Date")]
    public DateTime? CreateDate { get; set; }

    [Display(Name = "Updated By")]
    public string? UpdatedBy { get; set; }

    [Display(Name = "Update Date")]
    public DateTime? UpdateDate { get; set; }

    public int? Status { get; set; }

    [NotMapped]
    [FileExtension]
    public IFormFile? ImageUpload { get; set; }
}
