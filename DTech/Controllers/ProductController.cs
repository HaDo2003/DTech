using DTech.DAO;
using DTech.Models.EF;
using Microsoft.AspNetCore.Mvc;

namespace DTech.Controllers
{
    public class ProductController(
        BrandDAO brandDAO,
        CategoryDAO categoryDAO,
        ProductDAO productDAO
    ) : Controller
    {
        // Route: /laptop
        [HttpGet]
        public async Task<IActionResult> Category(string? categorySlug, string? sortOrder)
        {
            var products = new List<Product>();
            switch (categorySlug)
            {
                case "hot-sales":
                    products = await productDAO.GetDiscountedProductsAsync();
                    ViewBag.Title = "Hot Sales";
                    break;
                case "accessory":
                    products = await productDAO.GetAccessoriesAsync();
                    ViewBag.Title = "Accessories";
                    break;
                default:
                    var category = await categoryDAO.GetCategoryBySlugAsync(categorySlug);
                    if (category == null) return NotFound();

                    // Get category IDs: main + all its children
                    var categoryIds = new List<int> { category.CategoryId };

                    // Include subcategory IDs
                    if (category.InverseParent != null && category.InverseParent.Count != 0)
                    {
                        categoryIds.AddRange(category.InverseParent.Select(c => c.CategoryId));
                    }

                    products = await productDAO.GetProductsByCategoryIdAsync(categoryIds);
                    ViewBag.Title = category.Name;

                    break;
            }
            var brands = products
                .Where(p => p.Brand != null)
                .Select(p => p.Brand)
                .DistinctBy(b => b!.BrandId)
                .ToList();
            products = await productDAO.SortProducts(products, sortOrder);
            
            ViewData["CategorySlug"] = categorySlug;
            ViewBag.Brands = brands;
            ViewBag.SortOrder = sortOrder;
            return View("Category", products);
        }

        // Route: /laptop/acer
        [HttpGet]
        public async Task<IActionResult> CategoryBrand(string categorySlug, string brandSlug)
        {
            var products = new List<Product>();
            Category? category = new();
            Brand? brand = await brandDAO.GetBrandBySlugAsync(brandSlug);
            if (brand == null) return NotFound();

            switch (categorySlug)
            {
                case "hot-sales":
                    products = await productDAO.GetDiscountedProductsAsync(brand.BrandId);
                    var viewBagSlug = "Hot Sales";
                    ViewBag.Title = $"{brand.Name} {viewBagSlug}";
                    break;
                case "accessory":
                    products = await productDAO.GetAccessoriesAsync(brand.BrandId);
                    ViewBag.Title = "Accessories";
                    break;
                default:
                    category = await categoryDAO.GetCategoryBySlugAsync(categorySlug);
                    if (category == null) return NotFound();

                    // Get category IDs: main + all its children
                    var categoryIds = new List<int> { category.CategoryId };

                    // Include subcategory IDs
                    if (category.InverseParent != null && category.InverseParent.Count != 0)
                    {
                        categoryIds.AddRange(category.InverseParent.Select(c => c.CategoryId));
                    }

                    products = await productDAO.GetByCategoryAndBrandAsync(category.CategoryId, brand.BrandId);
                    ViewBag.Title = $"{brand.Name} {category.Name}";

                    break;
            }
            
            return View("Category", products);
        }

        // Route: /laptop/acer/acer-aspire
        [HttpGet]
        public async Task<IActionResult> ProductDetail(string categorySlug, string brandSlug, string productSlug)
        {
            try
            {
                var category = await categoryDAO.GetCategoryBySlugAsync(categorySlug);
                var brand = await brandDAO.GetBrandBySlugAsync(brandSlug);
                if (category == null || brand == null) return NotFound();

                var product = await productDAO.GetBySlugAsync(productSlug, category.CategoryId, brand.BrandId);
                if (product == null) return NotFound();
                await productDAO.IncreaseViewsAsync(product.ProductId);

                ViewBag.Breadcrumb = new[] { category.Name, brand.Name, product.Name };
                return View(product);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return NotFound();
            }
        }
    }
}
