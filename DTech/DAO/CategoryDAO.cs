using DTech.Models.EF;
using Microsoft.EntityFrameworkCore;

namespace DTech.DAO
{
    public class CategoryDAO
    (
        EcommerceWebContext context
    )
    {
        //Return all content of table
        public async Task<List<Category>> GetListAsync()
        {
            return await context.Categories
                .AsNoTracking()
                .Include(c => c.Parent)
                .ToListAsync();
        }

        //Return one row of table
        public async Task<Category?> GetByIdAsync(int? id)
        {
            if (id == null)
            {
                return null;
            }

            var category = await context.Categories
                .AsNoTracking()
                .Include(c => c.Parent)
                .FirstOrDefaultAsync(a => a.CategoryId == id);
            return category;
        }

        //Add new row to table
        public async Task<bool> AddAsync(Category category)
        {
            try
            {
                context.Categories.Add(category);
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
        public async Task<bool> UpdateAsync(Category category)
        {
            try
            {
                context.Categories.Update(category);
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
        public async Task<bool> DeleteAsync(Category category)
        {
            try
            {
                context.Categories.Remove(category);
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
            return await context.Categories.AnyAsync(e => e.CategoryId == id);
        }

        // Check Slug
        public async Task<Category?> CheckSlugAsync(string? slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return null;
            }

            var category = await context.Categories.FirstOrDefaultAsync(a => a.Slug == slug);
            return category;
        }

        // Check if the slug is already used by another category
        public async Task<Category?> CheckSlugAsync(string? newSlug, int? CategoryId)
        {
            if (string.IsNullOrEmpty(newSlug))
            {
                return null;
            }

            var category = await context.Categories
                .FirstOrDefaultAsync(a => a.Slug == newSlug && a.CategoryId != CategoryId);
            return category;
        }
    }
}
