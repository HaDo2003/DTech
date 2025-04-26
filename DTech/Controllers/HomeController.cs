using System.Diagnostics;
using DTech.DAO;
using DTech.Models;
using Microsoft.AspNetCore.Mvc;

namespace DTech.Controllers
{
    public class HomeController(
        AdvertisementDAO advertisementDAO
    ) : Controller
    {
        public async Task<IActionResult> Index()
        {
            ViewBag.Advertisements = await advertisementDAO.GetOrderedListAsync();

            return View();
        }
    }
}
