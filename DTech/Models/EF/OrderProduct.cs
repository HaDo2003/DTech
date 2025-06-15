using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DTech.Models.EF;

public partial class OrderProduct
{
    [Key]
    public int Id { get; set; }
    public int? OrderId { get; set; }

    public int? ProductId { get; set; }
    public decimal? InitialCost { get; set; }
    public decimal? Price { get; set; }

    public int? Quantity { get; set; }

    public decimal? CostAtPurchase { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
