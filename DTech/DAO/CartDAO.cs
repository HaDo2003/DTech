using DTech.Models.EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DTech.DAO
{
    public class CartDAO(
        EcommerceWebContext context
    ){
        public async Task CreateAsync(Cart cart)
        {
            context.Carts.Add(cart);
            await context.SaveChangesAsync();

            await Task.CompletedTask;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var cart = await context.Carts.FirstOrDefaultAsync(c => c.CustomerId == id);
            if (cart != null)
            {
                try
                {
                    context.Carts.Remove(cart);
                    await context.SaveChangesAsync();
                    return true;
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Handle concurrency exception
                    return false;
                }
                catch (DbUpdateException)
                {
                    // Handle update exception
                    return false;
                }
            }
            else
            {
                // Cart not found
                return false;
            }
        }
    }
}
