using DTech.Models.EF;
using Microsoft.EntityFrameworkCore;

namespace DTech.DAO
{
    public class PostCategoryDAO(
        EcommerceWebContext context
    ){
        //Return all content of table
        public async Task<List<PostCategory>> GetListAsync()
        {
            return await context.PostCategories.AsNoTracking().ToListAsync();
        }

        //Return one row of table
        public async Task<PostCategory?> GetByIdAsync(int? id)
        {
            if (id == null)
            {
                return null;
            }

            var postCategory = await context.PostCategories.AsNoTracking().FirstOrDefaultAsync(a => a.CategoryId == id);
            return postCategory;
        }

        //Add new row to table
        public async Task<bool> AddAsync(PostCategory postCategory)
        {
            try
            {
                context.PostCategories.Add(postCategory);
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
        public async Task<bool> UpdateAsync(PostCategory postCategory)
        {
            try
            {
                context.PostCategories.Update(postCategory);
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
        public async Task<bool> DeleteAsync(PostCategory postCategory)
        {
            try
            {
                context.PostCategories.Remove(postCategory);
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
            return await context.PostCategories.AnyAsync(e => e.CategoryId == id);
        }

        // Check Slug
        public async Task<PostCategory?> CheckSlugAsync(string? slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return null;
            }

            var postCategory = await context.PostCategories.FirstOrDefaultAsync(a => a.Slug == slug);
            return postCategory;
        }

        // Check if the slug is already used by another postCategory
        public async Task<PostCategory?> CheckSlugAsync(string? newSlug, int? cateId)
        {
            if (string.IsNullOrEmpty(newSlug))
            {
                return null;
            }

            var postCategory = await context.PostCategories
                .FirstOrDefaultAsync(a => a.Slug == newSlug && a.CategoryId != cateId);
            return postCategory;
        }
    }
}
