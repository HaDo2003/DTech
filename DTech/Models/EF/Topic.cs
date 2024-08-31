using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DTech.Models.EF;

public partial class Topic
{
    [Key]
    public int TopicId { get; set; }

    [Required(ErrorMessage = "Please enter name of topic")]
    public string? Name { get; set; }

    public string? Slug { get; set; }

    [Display(Name = "Parent")]
    public int? ParentId { get; set; }

    public int? Orders { get; set; }

    [Display(Name = "Created By")]
    public string? CreatedBy { get; set; }

    [Display(Name = "Create Date")]
    public DateTime? CreateDate { get; set; }

    [Display(Name = "Updated By")]
    public string? UpdatedBy { get; set; }

    [Display(Name = "Update Date")]
    public DateTime? UpdateDate { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<Topic> InverseParent { get; set; } = new List<Topic>();

    public virtual Topic? Parent { get; set; }
}
