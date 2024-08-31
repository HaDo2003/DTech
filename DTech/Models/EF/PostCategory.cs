using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DTech.Models.EF;

public partial class PostCategory
{
    [Key]
    public int CateId { get; set; }

    [Required(ErrorMessage = "Please enter name of post category")]
    public string? Name { get; set; }

    public string? Slug { get; set; }

    [Display(Name = "Created By")]
    public string? CreatedBy { get; set; }

    [Display(Name = "Create Date")]
    public DateTime? CreateDate { get; set; }

    [Display(Name = "Updated By")]
    public string? UpdatedBy { get; set; }

    [Display(Name = "Update Date")]
    public DateTime? UpdateDate { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
