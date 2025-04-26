using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DTech.Models.EF;

public partial class Cart
{
    [Key]
    public int CartId { get; set; }

    public string? CustomerId { get; set; }

    public virtual ApplicationUser? Customer { get; set; }
}
