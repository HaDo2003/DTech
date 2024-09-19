using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DTech.Models.EF;

public partial class Specification
{
    public int SpecId { get; set; }

    public int? ProductId { get; set; }

    [Display(Name = "Specification Name")]
    public string? SpecName { get; set; }

    public string? Slug { get; set; }

    public string? Detail { get; set; }

    public virtual Product? Product { get; set; }
}
