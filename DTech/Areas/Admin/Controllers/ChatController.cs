using DTech.DAO;
using DTech.Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DTech.Areas.Admin.Controllers
{
    [Area("Admin")]
    [SetViewBagAttributes]
    [Authorize(Roles = "Admin")]
    public class ChatController(
        ChatDAO chatDAO
    ) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Authentication");
            var chats = await chatDAO.GetChatPreviewsAsync(userId);
            return View(chats);
        }

        public async Task<IActionResult> FullChat(string senderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Authentication");
            var fullChat = await chatDAO.GetChatMessagesAsync(userId, senderId);
            return PartialView("FullChat", fullChat);
        }
    }
}
