using DTech.Models.EF;
using Microsoft.EntityFrameworkCore;

namespace DTech.DAO
{
    public class BrandDAO
    (
        EcommerceWebContext context
    ){
        //Return all content of table
        public async Task<List<Brand>> GetListAsync()
        {
            return await context.Brands.AsNoTracking().ToListAsync();
        }

        //Return one row of table
        public async Task<Brand?> GetByIdAsync(int? id)
        {
            if (id == null)
            {
                return null;
            }

            var brand = await context.Brands.AsNoTracking().FirstOrDefaultAsync(a => a.BrandId == id);
            return brand;
        }

        //Add new row to table
        public async Task<bool> AddAsync(Brand brand)
        {
            try
            {
                context.Brands.Add(brand);
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
        public async Task<bool> UpdateAsync(Brand brand)
        {
            try
            {
                context.Brands.Update(brand);
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
        public async Task<bool> DeleteAsync(Brand brand)
        {
            try
            {
                context.Brands.Remove(brand);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        //Check if id is valid
        public async Task<bool> CheckIdAsync(int? id)
        {
            return await context.Brands.AnyAsync(e => e.BrandId == id);
        }

        // Check Slug
        public async Task<Brand?> CheckSlugAsync(string? slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return null;
            }

            var brand = await context.Brands.FirstOrDefaultAsync(a => a.Slug == slug);
            return brand;
        }

        // Check if the slug is already used by another brand
        public async Task<Brand?> CheckSlugAsync(string? newSlug, int? brandId)
        {
            if (string.IsNullOrEmpty(newSlug))
            {
                return null;
            }

            var brand = await context.Brands
                .FirstOrDefaultAsync(a => a.Slug == newSlug && a.BrandId != brandId);
            return brand;
        }

        // Get brand by slug
        public async Task<Brand?> GetBrandBySlugAsync(string? slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return null;
            }
            var brand = await context.Brands
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Slug == slug);
            return brand;
        }
    }
}
