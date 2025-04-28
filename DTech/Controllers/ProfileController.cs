using Microsoft.AspNetCore.Mvc;

namespace DTech.Controllers
{
    public class ProfileController : Controller
    {
        public async Task<IActionResult> Profile()
        {
            return View();
        }
    }
}
