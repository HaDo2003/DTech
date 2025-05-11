using DTech.DAO;
using Microsoft.AspNetCore.Mvc;

namespace DTech.Controllers
{
    [Route("")]
    public class ProductController(
        BrandDAO brandDAO,
        CategoryDAO categoryDAO,
        ProductDAO productDAO
    ) : Controller
    {
        // Route: /laptop
        [HttpGet("{categorySlug}")]
        public async Task<IActionResult> Category(string categorySlug)
        {
            var category = await categoryDAO.GetCategoryBySlugAsync(categorySlug);
            if (category == null) return NotFound();

            // Get category IDs: main + all its children
            var categoryIds = new List<int> { category.CategoryId };

            // Include subcategory IDs
            if (category.InverseParent != null && category.InverseParent.Count != 0)
            {
                categoryIds.AddRange(category.InverseParent.Select(c => c.CategoryId));
            }

            var products = await productDAO.GetProductsByCategoryIdAsync(categoryIds);
            ViewBag.Title = category.Name;
            return View("Category", products);
        }

        // Route: /laptop/acer
        [HttpGet("{categorySlug}/{brandSlug}")]
        public async Task<IActionResult> CategoryBrand(string categorySlug, string brandSlug)
        {
            var category = await categoryDAO.GetCategoryBySlugAsync(categorySlug);
            var brand = await brandDAO.GetBrandBySlugAsync(brandSlug);
            if (category == null || brand == null) return NotFound();

            var products = await productDAO.GetByCategoryAndBrandAsync(category.CategoryId, brand.BrandId);
            ViewBag.Title = $"{brand.Name} {category.Name}";
            return View("Category", products);
        }

        // Route: /laptop/acer/acer-aspire
        [HttpGet("{categorySlug}/{brandSlug}/{productSlug}")]
        public async Task<IActionResult> Detail(string categorySlug, string brandSlug, string productSlug)
        {
            var category = await categoryDAO.GetCategoryBySlugAsync(categorySlug);
            var brand = await brandDAO.GetBrandBySlugAsync(brandSlug);
            if (category == null || brand == null) return NotFound();

            var product = await productDAO.GetBySlugAsync(productSlug, category.CategoryId, brand.BrandId);
            if (product == null) return NotFound();

            ViewBag.Breadcrumb = new[] { category.Name, brand.Name, product.Name };
            return View("Detail", product);
        }
    }
}
