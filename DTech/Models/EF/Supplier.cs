using System;
using System.Collections.Generic;

namespace DTech.Models.EF;

public partial class Supplier
{
    public int SupplierId { get; set; }

    public string? Name { get; set; }

    public string? Slug { get; set; }

    public string? Email { get; set; }

    public string? ResponsiblePerson { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? Description { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
