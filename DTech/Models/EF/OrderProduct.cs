using System;
using System.Collections.Generic;

namespace DTech.Models.EF;

public partial class OrderProduct
{
    public int? OrderId { get; set; }

    public int? ProductId { get; set; }

    public int? Quantity { get; set; }

    public int? CostAtPurchase { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }
}
