using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DTech.Models.EF;

public partial class PostComment
{
    [Key]
    public int CommentId { get; set; }

    public int? PostId { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Detail { get; set; }

    public DateTime? CmtDate { get; set; }

    public int? Status { get; set; }

    public virtual Post? Post { get; set; }
}
