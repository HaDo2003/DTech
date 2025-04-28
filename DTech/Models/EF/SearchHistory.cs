using System.ComponentModel.DataAnnotations;

namespace DTech.Models.EF
{
    public partial class SearchHistory
    {
        [Key]
        public int SearchHistoryId { get; set; }
        public string? UserId { get; set; }
        public string? SearchTerm { get; set; }
        public DateTime? SearchDate { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}
