using DTech.Models.EF;

namespace DTech.Models.ViewModel
{
    public class FullChatViewModel
    {
        public string? SenderId { get; set; }
        public string? SenderName { get; set; }
        public string? SenderImageUrl { get; set; }
        public List<Chat>? Messages { get; set; }
    }
}
