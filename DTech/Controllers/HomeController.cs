using System.Diagnostics;
using DTech.DAO;
using DTech.Models;
using DTech.Models.EF;
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

            var hotProducts = await productDAO.GetDiscountedProductsAsync();

            var accessoriesproduct = await productDAO.GetAccessoriesAsync();

            ViewBag.HotProducts = hotProducts;
            ViewBag.LaptopProducts = await GetProductsByCategory("Laptop");
            ViewBag.SmartphoneProducts = await GetProductsByCategory("Smart Phone");
            ViewBag.TabletProducts = await GetProductsByCategory("Tablet");
            ViewBag.AccessoriesProducts = accessoriesproduct;

            return View();
        }

        private async Task<List<Product>> GetProductsByCategory(string categoryName)
        {
            var categoryId = await categoryDAO.GetCategoryIdByNameAsync(categoryName);
            var products = await productDAO.GetProductsByCategoryIdAsync(categoryId);
            return products;
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
