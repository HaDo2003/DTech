using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DTech.Models.EF;

public partial class CartProduct
{
    [Key]
    public int Id { get; set; }
    public int? CartId { get; set; }

    public int? ProductId { get; set; }
    public int Quantity { get; set; }

    public virtual Cart? Cart { get; set; }

    public virtual Product? Product { get; set; }
}
