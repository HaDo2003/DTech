using DTech.Models.EF;
using Microsoft.EntityFrameworkCore;

namespace DTech.DAO
{
    public class ProductImageDAO(
        EcommerceWebContext context
    )
    {
        //Get product images by id
        public async Task<ProductImage?> GetProductImageByIdAsync(int? id)
        {
            if (id == null)
            {
                return null;
            }
            var productImage = await context.ProductImages.FindAsync(id);
            return productImage;
        }
        //Remove product image by id
        public async Task<bool> RemoveProductImageByIdAsync(int id)
        {
            try
            {
                var productImage = await context.ProductImages.FindAsync(id);
                if (productImage != null)
                {
                    context.ProductImages.Remove(productImage);
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
