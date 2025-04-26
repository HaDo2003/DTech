using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DTech.Models.EF;

public partial class CustomerAddress
{
    [Key]
    public int AddressId { get; set; }

    public string? CustomerId { get; set; }

    public string? Address { get; set; }

    public virtual ApplicationUser? Customer { get; set; }
}
