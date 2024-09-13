using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DTech.Models.EF;

public partial class ProductComment
{
    public int CommentId { get; set; }

    public int? ProductId { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Detail { get; set; }

    [Display(Name = "Comment Date")]
    public DateTime? CmtDate { get; set; }

    public decimal? Rate { get; set; }

    public virtual Product? Product { get; set; }
}
