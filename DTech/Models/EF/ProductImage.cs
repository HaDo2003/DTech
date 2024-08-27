using System;
using System.Collections.Generic;

namespace DTech.Models.EF;

public partial class ProductImage
{
    public int ProductId { get; set; }

    public string? Image { get; set; }

    public virtual Product Product { get; set; } = null!;
}
