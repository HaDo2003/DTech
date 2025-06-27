using DTech.Models.EF;
using DTech.Models.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace DTech.DAO
{
    
    public class ChatDAO(
        EcommerceWebContext context,
        RoleDAO roleDAO,
        AdminDAO adminDAO
    )
    {
        // Return all users of chat
        public async Task<List<ChatPreviewViewModel?>> GetChatPreviewsAsync(string adminId)
        {
            var chats = await context.Chats
                .Where(c => c.ReceiverId == adminId || c.SenderId == adminId)
                .Include(c => c.Sender)
                .Include(c => c.Receiver)
                .OrderByDescending(c => c.Timestamp)
                .AsNoTracking()
                .ToListAsync();

            // Group by the other user (customer)
            var previews = chats
                .GroupBy(c => c.SenderId == adminId ? c.ReceiverId : c.SenderId)
                .Select(g =>
                {
                    var latest = g.First();
                    var otherUser = latest.SenderId == adminId ? latest.Receiver : latest.Sender;
                    if (otherUser == null)
                    {
                        return null;
                    }

                    return new ChatPreviewViewModel
                    {
                        SenderId = otherUser.Id,
                        SenderName = otherUser.FullName,
                        Message = latest.Message,
                        Timestamp = latest.Timestamp,
                        AvatarUrl = otherUser.Image
                    };
                })
                .OrderByDescending(c => c.Timestamp)
                .ToList();

            return previews;
        }

        // Return all messages of chat with a specific user
        public async Task<FullChatViewModel> GetChatMessagesAsync(string adminId, string userId)
        {
             var chats = await context.Chats
                .Where(c => (c.SenderId == adminId && c.ReceiverId == userId) || (c.SenderId == userId && c.ReceiverId == adminId))
                .Include(c => c.Sender)
                .Include(c => c.Receiver)
                .OrderBy(c => c.Timestamp)
                .AsNoTracking()
                .ToListAsync();

            var otherUser = chats.FirstOrDefault(c => c.SenderId == userId || c.ReceiverId == userId)
                    ?.SenderId == adminId ? chats.First().Receiver : chats.First().Sender;

            var viewModel = new FullChatViewModel
            {
                SenderId = userId,
                SenderName = otherUser!.FullName,
                SenderImageUrl = otherUser.Image,
                Messages = chats
            };
            return viewModel;
        }

        // Return all messages of chat
        public async Task<FullChatViewModel?> GetChatMessagesAsync(string userId)
        {
            var adminId = "7aede655-1203-45b4-a00e-a0f81a812ecb";
            if (userId == adminId)
            {
                return null;
            }
            var chats = await context.Chats
               .Where(c => (c.SenderId == adminId && c.ReceiverId == userId) || (c.SenderId == userId && c.ReceiverId == adminId))
               .Include(c => c.Sender)
               .Include(c => c.Receiver)
               .OrderBy(c => c.Timestamp)
               .AsNoTracking()
               .ToListAsync();

            if (chats.Count == 0)
            {
                return null;
            }

            var adminUser = chats.FirstOrDefault(c => c.SenderId == userId || c.ReceiverId == userId)
                ?.SenderId == adminId ? chats.First().Receiver : chats.First().Sender;

            var viewModel = new FullChatViewModel
            {
                SenderId = userId,
                SenderName = adminUser!.FullName,
                SenderImageUrl = adminUser.Image,
                Messages = chats
            };
            return viewModel;
        }
    }
}
