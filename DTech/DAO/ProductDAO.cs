using DTech.Models.EF;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace DTech.DAO
{
    public class ProductDAO(
        EcommerceWebContext context
    )
    {
        //Return all content of table
        public async Task<List<Product>> GetListAsync()
        {
            return await context.Products
                .AsNoTracking()
                .Include(a => a.Brand)
                .Include(a => a.Category)
                .Include(a => a.Supplier)
                .ToListAsync();
        }

        //Return all content of table with ids
        public async Task<List<Product>> GetByIdsAsync(List<int> ids)
        {
            return await context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p => ids.Contains(p.ProductId)
                            && p.Brand != null
                            && p.Category != null)
                .ToListAsync();
        }

        //Return one row of table
        public async Task<Product?> GetByIdAsync(int? id)
        {
            if (id == null)
            {
                return null;
            }
            var product = await context.Products
                .Include(a => a.Brand)
                .Include(a => a.Category)
                .Include(a => a.Supplier)
                .Include(a => a.ProductImages)
                .Include(a => a.Specifications)
                .FirstOrDefaultAsync(a => a.ProductId == id);
            return product;
        }
        //Add new row to table
        public async Task<bool> AddAsync(Product product)
        {
            try
            {
                context.Products.Add(product);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        //Update row in table
        public async Task<bool> UpdateAsync(Product product)
        {
            try
            {
                context.Products.Update(product);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        //Delete row in table
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var product = await GetByIdAsync(id);
                if (product != null)
                {
                    var result = await RemoveAllSpecificationsAsync(id);
                    if (result)
                    {
                        context.Products.Remove(product);
                        await context.SaveChangesAsync();
                        return true;
                    }
                    return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        //Check if id exists
        public async Task<bool> CheckIdAsync(int? id)
        {
            return await context.Products.AnyAsync(e => e.ProductId == id);
        }

        //Check if slug exists
        public async Task<bool> CheckSlugAsync(string? slug)
        {
            return await context.Products.AnyAsync(a => a.Slug == slug);
        }

        public async Task<Product?> CheckSlugAsync(string? slug, int? id)
        {
            if (slug == null)
            {
                return null;
            }
            var product = await context.Products.AsNoTracking()
                .FirstOrDefaultAsync(a => a.Slug == slug && a.ProductId != id);
            return product;
        }

        //Remove all specifications of a product
        public async Task<bool> RemoveAllSpecificationsAsync(int? id)
        {
            try
            {
                var specifications = await context.Specifications
                    .Where(a => a.ProductId == id)
                    .ToListAsync();
                if (specifications.Count != 0)
                {
                    await context.BulkDeleteAsync(specifications);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        //Save changes
        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        //Get all products by category id
        public async Task<List<Product>> GetProductsByCategoryIdAsync(List<int> id)
        {
            if (id == null)
            {
                return [];
            }
            var products = await context.Products
                .AsNoTracking()
                .Include(a => a.Brand)
                .Include(a => a.Category)
                .Include(a => a.Supplier)
                .Where(p => p.CategoryId != null && id.Contains(p.CategoryId.Value) && p.Status == 1)
                .OrderByDescending(a => a.ProductId)
                .ToListAsync();
            return products;
        }

        //Get all products by category id and brand id
        public async Task<List<Product>> GetByCategoryAndBrandAsync(int? categoryId, int? brandId)
        {
            if (categoryId == null || brandId == null)
            {
                return [];
            }
            var products = await context.Products
                .AsNoTracking()
                .Include(a => a.Brand)
                .Include(a => a.Category)
                .Include(a => a.Supplier)
                .Where(a => a.CategoryId == categoryId && a.BrandId == brandId && a.Status == 1)
                .OrderByDescending(a => a.ProductId)
                .ToListAsync();
            return products;
        }

        //Get a product by slug, category id and brand id
        public async Task<Product?> GetBySlugAsync(string? slug, int? categoryId, int? brandId)
        {
            if (slug == null || categoryId == null || brandId == null)
            {
                return null;
            }
            var product = await context.Products
                .AsNoTracking()
                .Include(a => a.Brand)
                .Include(a => a.Category)
                .Include(a => a.Supplier)
                .Include(a => a.ProductImages)
                .Include(a => a.Specifications)
                //.FirstOrDefaultAsync(a => a.Slug == slug && a.CategoryId == categoryId && a.BrandId == brandId && a.Status == 1);
                .FirstOrDefaultAsync(a => a.Slug == slug && a.Status == 1);
            return product;
        }

        //Get all accessories
        public async Task<List<Product>> GetAccessoriesAsync()
        {
            var products = await context.Products
                .AsNoTracking()
                .Include(a => a.Brand)
                .Include(a => a.Category)
                .Include(a => a.Supplier)
                .Where(a => a.Category!.Name != "Laptop" && a.Category!.Name != "Smart Phone" && a.Category!.Name != "Tablet" && a.Status == 1)
                .ToListAsync();

            // Shuffle the list randomly
            var random = new Random();
            return [.. products.OrderBy(p => random.Next())];
        }

        public async Task<List<Product>> GetAccessoriesAsync(int brandId)
        {
            var products = await context.Products
                .AsNoTracking()
                .Include(a => a.Brand)
                .Include(a => a.Category)
                .Include(a => a.Supplier)
                .Where(a => a.Category!.Name != "Laptop" 
                    && a.Category!.Name != "Smart Phone" 
                    && a.Category!.Name != "Tablet"
                    && a.BrandId == brandId
                    && a.Status == 1)
                .ToListAsync();

            // Shuffle the list randomly
            var random = new Random();
            return [.. products.OrderBy(p => random.Next())];
        }

        //Get all products that are discounted
        public async Task<List<Product>> GetDiscountedProductsAsync()
        {
            var products = await context.Products
                .AsNoTracking()
                .Include(a => a.Brand)
                .Include(a => a.Category)
                .Include(a => a.Supplier)
                .Where(a => a.Discount != null && a.Discount > 0 && a.Status == 1)
                .OrderByDescending(a => a.Discount)
                .ToListAsync();
            return products;
        }

        public async Task<List<Product>> GetDiscountedProductsAsync(int brandId)
        {
            var products = await context.Products
                .AsNoTracking()
                .Include(a => a.Brand)
                .Include(a => a.Category)
                .Include(a => a.Supplier)
                .Where(a => a.Discount != null && a.Discount > 0 && a.BrandId == brandId && a.Status == 1)
                .OrderByDescending(a => a.Discount)
                .ToListAsync();
            return products;
        }

        //Sort products
        public Task<List<Product>> SortProducts(List<Product> products, string? sortOrder)
        {
            return Task.FromResult(sortOrder switch
            {
                "newest" => [.. products.Where(a => a.Status == 1).OrderBy(p => p.ProductId)],
                "discount" => [.. products.Where(a => a.Status == 1).OrderByDescending(p => p.Discount)],
                "name_asc" => [.. products.Where(a => a.Status == 1).OrderBy(p => p.Name)],
                "name_desc" => [.. products.Where(a => a.Status == 1).OrderByDescending(p => p.Name)],
                "price_asc" => [.. products.Where(a => a.Status == 1).OrderBy(p => p.Price)],
                "price_desc" => [.. products.Where(a => a.Status == 1).OrderByDescending(p => p.Price)],
                _ => products
            });
        }

        //Update product views
        public async Task<bool> IncreaseViewsAsync(int? id)
        {
            try
            {
                var product = await context.Products.FindAsync(id);
                if (product != null)
                {
                    product.Views = (product.Views ?? 0) + 1;
                    context.Products.Update(product);
                    await context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        //Get related products by brand
        public async Task<List<Product>> GetRelatedProductsAsync(int? brandId, int? productId)
        {
            if (brandId == null || productId == null)
            {
                return [];
            }
            var products = await context.Products
                .AsNoTracking()
                .Include(a => a.Brand)
                .Include(a => a.Category)
                .Include(a => a.Supplier)
                .Where(a => a.BrandId == brandId && a.ProductId != productId && a.Status == 1)
                .OrderByDescending(a => a.ProductId)
                .Take(5)
                .ToListAsync();
            return products;
        }

    }
}
