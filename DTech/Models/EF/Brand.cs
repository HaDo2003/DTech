using System;
using System.Collections.Generic;

namespace DTech.Models.EF;

public partial class Brand
{
    public int BrandId { get; set; }

    public string? Name { get; set; }

    public string? Slug { get; set; }

    public string? Logo { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
