using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DTech.Models.EF;

public partial class PaymentMethod
{
    [Display(Name = "ID")]
    public int PaymentMethodId { get; set; }

    public string? Description { get; set; }

    [Display(Name = "Created By")]
    public string? CreatedBy { get; set; }

    [Display(Name = "Create Date")]
    public DateTime? CreateDate { get; set; }

    [Display(Name = "Updated By")]
    public string? UpdatedBy { get; set; }

    [Display(Name = "Created Date")]
    public DateTime? UpdateDate { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
