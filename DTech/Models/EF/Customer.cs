﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTech.Models.EF;

public partial class Customer
{
    [Key]
    public int CustomerId { get; set; }

    public int CartId { get; set; }

    [Required(ErrorMessage = "Please enter first name")]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "Please enter last name")]
    public string? LastName { get; set; }

    [Required(ErrorMessage = "Please check gender")]
    public string? Gender { get; set; }

    [Required(ErrorMessage = "Please enter Day Of Birth")]
    public DateOnly? DayOfBirth { get; set; }

    [Required(ErrorMessage = "Please enter phone number")]
    [Phone(ErrorMessage = "Invalid phone number")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Please enter email")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Please enter account")]
    [MinLength(6, ErrorMessage = "Account must have at least 6 characters ")]
    public string? Account { get; set; }

    [Required(ErrorMessage = "Please enter password")]
    [MinLength(6, ErrorMessage = "Password must have at least 6 characters ")]
    public string? Password { get; set; }

    public string? Image { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual ICollection<CustomerCoupon> CustomerCoupons { get; set; } = new List<CustomerCoupon>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [NotMapped]
    [FileExtensions]
    public IFormFile? ImageUpload { get; set; }
}
