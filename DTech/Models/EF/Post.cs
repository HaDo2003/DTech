using System;
using System.Collections.Generic;

namespace DTech.Models.EF;

public partial class Post
{
    public int PostId { get; set; }

    public int CateId { get; set; }

    public string? Name { get; set; }

    public string? Slug { get; set; }

    public string? Image { get; set; }

    public string? Description { get; set; }

    public DateTime? PostDate { get; set; }

    public string? PostBy { get; set; }

    public int? Status { get; set; }

    public virtual PostCategory Cate { get; set; } = null!;

    public virtual ICollection<PostComment> PostComments { get; set; } = new List<PostComment>();
}
