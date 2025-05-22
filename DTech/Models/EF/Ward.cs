using System.ComponentModel.DataAnnotations;

namespace DTech.Models.EF
{
    public class Ward
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public int DistrictId { get; set; }
        public virtual District? District { get; set; } = null!;
    }
}
