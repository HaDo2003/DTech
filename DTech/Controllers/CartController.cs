using DTech.DAO;
using DTech.Models.EF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DTech.Controllers
{
    [Authorize]
    [Route("cart")]
    public class CartController(
        CartDAO cartDAO,
        ProductDAO productDAO
    ) : Controller
    {
        // Route: /cart
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Authentication");

            var usercart = await cartDAO.GetCartByUserId(userId);
            if (usercart == null)
                return NotFound();

            return View(usercart);
        }

        // Route: /cart/add
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add")]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Authentication required", notLoggedIn = true });
            }

            var product = await productDAO.GetByIdAsync(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Product does not exist." });
            }

            var cart = await cartDAO.GetCartByUserId(userId);
            if (cart == null)
            {
                return Json(new { success = false, message = "Cart not found." });
            }

            var result = await cartDAO.AddToCart(cart.CartId, productId, quantity);
            if (result)
            {
                int cartCount = await cartDAO.GetCartItemCount(cart.CartId);
                return Json(new { success = true, cartCount });
            }

            return Json(new { success = false, message = "Failed to add to cart." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("update-quantity")]
        public async Task<IActionResult> UpdateQuantity(int cartProductId, int quantity)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "Authentication required", notLoggedIn = true });
                }

                var totalItemPrice = await cartDAO.UpdateQuantity(cartProductId, quantity, userId);

                if (totalItemPrice < 0)
                {
                    return Json(new { success = false, message = "Cart product not found" });
                }

                var cartTotal = await cartDAO.GetCartTotal(userId);
                var cart = await cartDAO.GetCartByUserId(userId);
                if (cart == null)
                {
                    return Json(new { success = false, message = "Cart not found." });
                }
                var cartCount = await cartDAO.GetCartItemCount(cart.CartId);

                var result = new
                {
                    success = true,
                    itemTotal = ((decimal)totalItemPrice!).ToString("N0"),
                    cartTotal = ((decimal)cartTotal!).ToString("N0"),
                    cartCount
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Route: /cart/remove
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int cartProductId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Authentication");
            var cart = await cartDAO.GetCartByUserId(userId);
            if (cart == null)
            {
                return Json(new { success = false, message = "Cart not found." });
            }
            var result = await cartDAO.RemoveFromCart(cartProductId, cart.CartId);
            if (result)
            {
                _ = await cartDAO.GetCartItemCount(cart.CartId);
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}
