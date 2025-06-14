using DTech.DAO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTech.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Seller")]
    public class DashboardController(
        ProductDAO productDAO,
        OrderDAO orderDAO
    ) : Controller
    {
        public async Task<IActionResult> Index()
        {
            ViewBag.Products = await productDAO.GetListOrderByIdAsync();
            ViewBag.Orders = await orderDAO.GetListByDecendingAsync();
            return View();
        }
    }
}
