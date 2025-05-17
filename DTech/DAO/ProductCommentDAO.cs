using DTech.Models.EF;
using Microsoft.EntityFrameworkCore;

namespace DTech.DAO
{
    public class ProductCommentDAO(
        EcommerceWebContext context
    )
    {
        //Get all comments by product id
        public async Task<List<ProductComment>> GetCommentsByProductIdAsync(int productId)
        {
            var comments = await context.ProductComments
                .AsNoTracking()
                .Where(c => c.ProductId == productId)
                .ToListAsync();
            return comments;
        }
        //Add new comment to table
        public async Task<bool> AddCommentAsync(ProductComment comment)
        {
            try
            {
                context.ProductComments.Add(comment);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        //Remove comment by id
        public async Task<bool> RemoveCommentByIdAsync(int id)
        {
            try
            {
                var comment = await context.ProductComments.FindAsync(id);
                if (comment != null)
                {
                    context.ProductComments.Remove(comment);
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
    }
}
