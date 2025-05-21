using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTech.Controllers
{
    [Authorize]
    [Route("checkout")]
    public class CheckOutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
