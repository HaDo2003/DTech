using System;
using System.Collections.Generic;

namespace DTech.Models.EF;

public partial class Menu
{
    public int MenuId { get; set; }

    public string? Name { get; set; }

    public string? Link { get; set; }

    public int? TableId { get; set; }

    public string? TypeMenu { get; set; }

    public string? Position { get; set; }

    public int? ParentId { get; set; }

    public int? Orders { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<Menu> InverseParent { get; set; } = new List<Menu>();

    public virtual Menu? Parent { get; set; }
}
