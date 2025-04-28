using System.ComponentModel.DataAnnotations;

namespace DTech.Models.EF
{
    public partial class Quiz
    {
        [Key]
        public int QuizId { get; set; }
        public string? Description { get; set; }
        public string? Answer1 { get; set; }
        public string? Answer2 { get; set; }
        public string? Answer3 { get; set; }
        public string? Answer4 { get; set; }
        public string? CorrectAnswer { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? Status { get; set; }
    }
}
