using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DTech.Models.EF;

public partial class Cart
{
    [Key]
    public int CartId { get; set; }

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
}
