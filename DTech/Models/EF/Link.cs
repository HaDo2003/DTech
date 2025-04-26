using System;
using System.Collections.Generic;

namespace DTech.Models.EF;

public partial class Link
{
    public int LinkId { get; set; }

    public string? Slug { get; set; }

    public string? TypeLink { get; set; }

    public int? TableId { get; set; }
}
