using DTech.DAO;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DTech.Controllers
{
    public class ChatController(ChatDAO chatDAO) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId)) { 
                var fullChat = await chatDAO.GetChatMessagesAsync(userId);
                return PartialView("~/Views/Shared/_ChatBox.cshtml", fullChat);
            }
                
            return PartialView("~/Views/Shared/_ChatBox.cshtml");
        }
    }
}
