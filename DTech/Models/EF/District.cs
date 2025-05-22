using System.ComponentModel.DataAnnotations;

namespace DTech.Models.EF
{
    public partial class District
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public int ProvinceId { get; set; }
        public virtual Province? Province { get; set; } = null!;
        public ICollection<Ward>? Wards { get; set; }
    }
}
