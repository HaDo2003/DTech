using Microsoft.AspNetCore.Mvc;

namespace DTech.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
