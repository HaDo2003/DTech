using System;
using System.Collections.Generic;

namespace DTech.Models.EF;

public partial class CustomerAddress
{
    public int CustomerId { get; set; }

    public string? Address { get; set; }

    public virtual Customer Customer { get; set; } = null!;
}
