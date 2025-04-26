using DTech.Library.Validation;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTech.Models.EF;

public partial class ApplicationUser: IdentityUser
{
    public string RoleId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please choose gender")]
    public string? Gender { get; set; }

    [Required(ErrorMessage = "Please enter Date of Birth")]
    [Display(Name = "Date of Birth")]
    public DateOnly? DateOfBirth { get; set; }

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

    [NotMapped]
    public string? RoleName { get; set; }
}
