using DTech.Models.EF;
using Microsoft.EntityFrameworkCore;

namespace DTech.DAO
{
    public class PostDAO(
        EcommerceWebContext context
    ){
        //Return all content of table
        public async Task<List<Post>> GetListAsync()
        {
            return await context.Posts
                .AsNoTracking()
                .Include(p => p.Cate)
                .ToListAsync();
        }

        //Return one row of table
        public async Task<Post?> GetByIdAsync(int? id)
        {
            if (id == null)
            {
                return null;
            }

            var post = await context.Posts
                .AsNoTracking()
                .Include(p => p.Cate)
                .FirstOrDefaultAsync(a => a.PostId == id);
            return post;
        }

        //Add new row to table
        public async Task<bool> AddAsync(Post post)
        {
            try
            {
                context.Posts.Add(post);
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
        public async Task<bool> UpdateAsync(Post post)
        {
            try
            {
                context.Posts.Update(post);
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
        public async Task<bool> DeleteAsync(Post post)
        {
            try
            {
                context.Posts.Remove(post);
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
            return await context.Posts.AnyAsync(e => e.PostId == id);
        }

        // Check Slug
        public async Task<Post?> CheckSlugAsync(string? slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return null;
            }

            var post = await context.Posts.FirstOrDefaultAsync(a => a.Slug == slug);
            return post;
        }

        // Check if the slug is already used by another post
        public async Task<Post?> CheckSlugAsync(string? newSlug, int? PostId)
        {
            if (string.IsNullOrEmpty(newSlug))
            {
                return null;
            }

            var post = await context.Posts
                .FirstOrDefaultAsync(a => a.Slug == newSlug && a.PostId != PostId);
            return post;
        }
    }
}
