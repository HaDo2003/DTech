using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTech.Models.EF;

public partial class ProductImage
{
    [Key]
    public int ImageId { get; set; }

    public int? ProductId { get; set; }

    public string? Image { get; set; }

    public virtual Product? Product { get; set; }

    [NotMapped]
    public IFormFile? ImageUpload {  get; set; }
}
