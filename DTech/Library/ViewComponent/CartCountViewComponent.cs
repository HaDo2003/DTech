using DTech.DAO;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public class CartCountViewComponent(CartDAO cartDAO) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        int cartCount = 0;

        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userId))
        {
            var cart = await cartDAO.GetCartByUserId(userId);
            if (cart != null)
            {
                cartCount = await cartDAO.GetCartItemCount(cart.CartId);
            }
        }

        return View(cartCount);
    }
}
