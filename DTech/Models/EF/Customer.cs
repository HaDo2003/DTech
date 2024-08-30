using DTech.Library.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTech.Models.EF;

public partial class Customer
{
    [Key]
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "Please enter first name")]
    [Display(Name = "First Name")]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "Please enter last name")]
    [Display(Name = "Last Name")]
    public string? LastName { get; set; }

    [Required(ErrorMessage = "Please check gender")]
    public string? Gender { get; set; }

    [Required(ErrorMessage = "Please enter date of birth")]
    [Display(Name = "Date Of Birth")]
    public DateOnly? DayOfBirth { get; set; }

    [Required(ErrorMessage = "Please enter phone number")]
    [Display(Name = "Phone Number")]
    [Phone]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Please enter email address")]
    [Display(Name = "Email Address")]
    [EmailAddress]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Please enter account")]
    [MinLength(6, ErrorMessage = "Account must have at least 6 characters ")]
    public string? Account { get; set; }

    [Required(ErrorMessage = "Please enter password")]
    [MinLength(6, ErrorMessage = "Password must have at least 6 characters ")]
    public string? Password { get; set; }

    public string? Image { get; set; }

    [Display(Name = "Created By")]
    public string? CreatedBy { get; set; }

    [Display(Name = "Create Date")]
    public DateTime? CreateDate { get; set; }

    [Display(Name = "Updated By")]
    public string? UpdatedBy { get; set; }

    [Display(Name = "Update Date")]
    public DateTime? UpdateDate { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<CustomerAddress> CustomerAddresses { get; set; } = new List<CustomerAddress>();

    public virtual ICollection<CustomerCoupon> CustomerCoupons { get; set; } = new List<CustomerCoupon>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [NotMapped]
    [FileExtension]
    public IFormFile? ImageUpload { get; set; }

    [NotMapped]
    public string? Address { get; set; }
}
