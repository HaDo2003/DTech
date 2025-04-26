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
        public IActionResult Index()
        {
            ViewBag.Advertisements = advertisementDAO.GetOrderedListAsync().Result;

            return View();
        }
    }
}
