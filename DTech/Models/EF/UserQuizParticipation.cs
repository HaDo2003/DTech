using System.ComponentModel.DataAnnotations;

namespace DTech.Models.EF
{
    public partial class UserQuizParticipation
    {
        [Key]
        public int UserQuizParticipationId { get; set; }
        public string? UserId { get; set; }
        public int? QuizId { get; set; }
        public string? SelectedAnswer { get; set; }
        public DateTime? ParticipationDate { get; set; }
        public bool IsCorrect { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public virtual Quiz? Quiz { get; set; }
    }
}
