using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTech.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Seller")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
