using System.ComponentModel.DataAnnotations;

namespace DTech.Models.EF
{
    public partial class WishList
    {
        [Key]
        public int WishListId { get; set; }
        public string? CustomerId { get; set; }

        public int? ProductId { get; set; }

        public virtual ApplicationUser? Customer { get; set; }

        public virtual Product? Product { get; set; }
    }
}
