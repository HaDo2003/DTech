using DTech.Models.EF;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.InteractiveChat;

namespace DTech.Library.Hubs
{
    public class ChatsHub(EcommerceWebContext context) : Hub
    {
        public async Task SendMessage(string receiverId, string message)
        {
            var senderId = Context.UserIdentifier;
            if (senderId == null)
            {
                senderId = null;
            }

            var chat = new Chat
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Message = message,
                Timestamp = DateTime.UtcNow
            };
            context.Chats.Add(chat);
            await context.SaveChangesAsync();

            await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, message);
        }
    }
}
