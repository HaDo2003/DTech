// DTech/ViewComponents/ChatBoxViewComponent.cs
using Microsoft.AspNetCore.Mvc;
using DTech.DAO;
using System.Security.Claims;

public class ChatBoxViewComponent(ChatDAO chatDAO) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userId))
        {
            var fullChat = await chatDAO.GetChatMessagesAsync(userId);
            return View("~/Views/Shared/Components/ChatBox/_ChatBox.cshtml", fullChat);
        }
        return View("~/Views/Shared/Components/ChatBox/_ChatBox.cshtml");
    }
}

