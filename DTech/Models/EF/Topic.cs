using System;
using System.Collections.Generic;

namespace DTech.Models.EF;

public partial class Topic
{
    public int TopicId { get; set; }

    public string? Name { get; set; }

    public string? Slug { get; set; }

    public int? ParentId { get; set; }

    public int? Orders { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public int? Status { get; set; }
}
