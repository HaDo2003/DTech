using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DTech.Models.EF;

public partial class Supplier
{
    [Key]
    public int SupplierId { get; set; }

    [Required(ErrorMessage = "Please enter name of supplier")]
    public string? Name { get; set; }

    public string? Slug { get; set; }

    [Required(ErrorMessage = "Please enter email of supplier")]
    [EmailAddress]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Please enter name of responsible person")]
    [Display(Name = "Responsible Person")]
    public string? ResponsiblePerson { get; set; }

    [Required(ErrorMessage = "Please enter phone number of supplier")]
    [Phone]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Please enter address of supplier")]
    public string? Address { get; set; }

    public string? Description { get; set; }

    [Display(Name = "Update Date")]
    public DateTime? UpdateDate { get; set; }

    [Display(Name = "Created By")]
    public string? CreatedBy { get; set; }

    [Display(Name = "Create Date")]
    public DateTime? CreateDate { get; set; }

    [Display(Name = "Updated By")]
    public string? UpdatedBy { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
