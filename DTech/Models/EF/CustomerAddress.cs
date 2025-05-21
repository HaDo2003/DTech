using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DTech.Models.EF;

public partial class CustomerAddress
{
    [Key]
    public int AddressId { get; set; }

    public string? CustomerId { get; set; }

    [Display(Name = "Full Name")]
    [StringLength(100, ErrorMessage = "Full Name must be less than 100 characters")]
    public string? FullName { get; set; }

    [Display(Name = "Phone Number")]
    [StringLength(15, ErrorMessage = "Phone Number must be less than 15 characters")]
    [RegularExpression(@"^\d{10,15}$", ErrorMessage = "Phone Number must be a valid number")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Province")]
    public string? Province { get; set; }

    [Display(Name = "District")]
    public string? District { get; set; }

    [Display(Name = "Ward")]
    public string? Ward { get; set; }

    [Display(Name = "Address (Street, House No., etc.)")]
    public string? Address { get; set; }

    public bool IsDefault { get; set; } = false;

    public virtual ApplicationUser? Customer { get; set; }
}
