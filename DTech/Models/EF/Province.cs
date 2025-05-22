using System.ComponentModel.DataAnnotations;

namespace DTech.Models.EF
{
    public partial class Province
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public ICollection<District>? Districts { get; set; }
    }
}
