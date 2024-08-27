using System;
using System.Collections.Generic;

namespace DTech.Models.EF;

public partial class PostCategory
{
    public int CateId { get; set; }

    public string? Name { get; set; }

    public string? Slug { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
