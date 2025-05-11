using System.Diagnostics;
using DTech.DAO;
using DTech.Models;
using Microsoft.AspNetCore.Mvc;

namespace DTech.Controllers
{
    public class HomeController(
        AdvertisementDAO advertisementDAO,
        ProductDAO productDAO,
        CategoryDAO categoryDAO
    ) : Controller
    {
        public async Task<IActionResult> Index()
        {
            ViewBag.Advertisements = await advertisementDAO.GetOrderedListAsync();

            var laptopCategoryId = await categoryDAO.GetCategoryIdByNameAsync("Laptop");
            var laptopProducts = await productDAO.GetProductsByCategoryIdAsync(laptopCategoryId);

            var smartPhoneCategoryId = await categoryDAO.GetCategoryIdByNameAsync("Smart Phone");
            var smartPhoneProducts = await productDAO.GetProductsByCategoryIdAsync(smartPhoneCategoryId);

            var hotProducts = await productDAO.GetDiscountedProductsAsync();

            ViewBag.HotProducts = hotProducts;
            ViewBag.LaptopProducts = laptopProducts;
            ViewBag.SmartphoneProducts = smartPhoneProducts;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statuscode)
        {
            if (statuscode == 404)
            {
                return View("NotFound");
            }
            else
            {
                return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
            }
        }
    }
}
