using DTech.Library.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DTech.Models.EF;

public partial class Admin
{
    [Key]
    public int AdminId { get; set; }

    [Required(ErrorMessage = "Please enter account")]
    [MinLength(6, ErrorMessage = "Account must have a least 6 characters")]
    [MaxLength(50, ErrorMessage = "Account can only have a maximum of 50 characters")]
    public string? Account { get; set; }

    [Required(ErrorMessage = "Please enter password")]
    [MinLength(6, ErrorMessage = "Password must have a least 6 characters")]
    [MaxLength(50, ErrorMessage = "Password can only have a maximum of 50 characters")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Please enter first name")]
    [Display(Name = "First Name")]
    [MaxLength(50, ErrorMessage = "First Name can only have a maximum of 50 characters ")]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "Please enter last name")]
    [Display(Name = "Last Name")]
    [MaxLength(50, ErrorMessage = "Last Name can only have a maximum of 50 characters ")]
    public string? LastName { get; set; }

    [Required(ErrorMessage = "Please check gender")]
    public string? Gender { get; set; }

    public string? Photo { get; set; }

    [Display(Name = "Created By")]
    public string? CreatedBy { get; set; }

    [Display(Name = "Create Date")]
    public DateTime? CreateDate { get; set; }

    [Display(Name = "Updated By")]
    public string? UpdatedBy { get; set; }

    [Display(Name = "Update Date")]
    public DateTime? UpdateDate { get; set; }

    [NotMapped]
    [FileExtension]
    public IFormFile? PhotoUpload { get; set; }
}
