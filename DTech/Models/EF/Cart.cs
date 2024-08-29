using System;
using System.Collections.Generic;

namespace DTech.Models.EF;

public partial class Cart
{
    public int CartId { get; set; }

    public virtual Customer? Customer { get; set; }
}
