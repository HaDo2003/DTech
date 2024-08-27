using System;
using System.Collections.Generic;

namespace DTech.Models.EF;

public partial class CartProduct
{
    public int CartId { get; set; }

    public int ProductId { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
