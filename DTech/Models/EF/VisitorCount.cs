using System.ComponentModel.DataAnnotations;

namespace DTech.Models.EF
{
    public partial class VisitorCount
    {
        [Key]
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public int Count { get; set; }
    }
}
