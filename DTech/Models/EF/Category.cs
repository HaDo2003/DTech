using System;
using System.Collections.Generic;

namespace DTech.Models.EF;

public partial class Category
{
    public int CategoryId { get; set; }

    public string? Name { get; set; }

    public int? ParentId { get; set; }

    public string? Slug { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<Category> InverseParent { get; set; } = new List<Category>();

    public virtual Category? Parent { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
