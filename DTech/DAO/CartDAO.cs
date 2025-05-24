using DTech.Models.EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Charts;

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

        public async Task<Cart?> GetCartByUserId(string userId)
        {
            var cart = await context.Carts
                .Include(c => c.CartProducts)
                .ThenInclude(cp => cp.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == userId);
            return cart;
        }

        public async Task<bool> AddToCart(int cartId, int productId, int quantity)
        {
            var cart = await context.Carts
                .Include(c => c.CartProducts)
                .FirstOrDefaultAsync(c => c.CartId == cartId);

            if (cart != null)
            {
                var existingProduct = cart.CartProducts
                    .FirstOrDefault(cp => cp.ProductId == productId);

                if (existingProduct != null)
                {
                    existingProduct.Quantity += quantity;
                }
                else
                {
                    context.CartProducts.Add(new CartProduct
                    {
                        CartId = cartId,
                        ProductId = productId,
                        Quantity = quantity
                    });
                }

                await context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        public async Task<int> GetCartItemCount(int cartId)
        {
            return await context.CartProducts
                .Where(cp => cp.CartId == cartId)
                .SumAsync(cp => cp.Quantity);
        }

        public async Task<decimal?> UpdateQuantity(int cartProductId, int newQuantity, string userId)
        {
            var cartProduct = await context.CartProducts
                .Include(cp => cp.Cart)
                .Include(cp => cp.Product)
                .FirstOrDefaultAsync(cp =>
                    cp.Id == cartProductId &&
                    cp.Cart != null &&
                    cp.Cart.CustomerId == userId);

            if (cartProduct == null) return -1;

            cartProduct.Quantity = newQuantity;
            await context.SaveChangesAsync();

            var discount = cartProduct.Product!.Discount ?? 0;
            var unitPrice = cartProduct.Product.Price * (1 - discount / 100m);
            return unitPrice * newQuantity;
        }

        public async Task<decimal?> GetCartTotal(string userId)
        {
            return await context.CartProducts
                .Where(cp => cp.Cart != null && cp.Cart.CustomerId == userId)
                .SumAsync(cp => cp.Quantity * cp.Product!.Price * (cp.Product.Discount.HasValue ? (1 - cp.Product.Discount.Value / 100m) : 1));
        }

        public async Task<bool> RemoveFromCart(int cartProductId, int Cartid)
        {
            var cartProduct = await context.CartProducts
                .Include(cp => cp.Cart)
                .FirstOrDefaultAsync(cp =>
                    cp.ProductId == cartProductId &&
                    cp.Cart != null &&
                    cp.Cart.CartId == Cartid);
            if (cartProduct != null)
            {
                context.CartProducts.Remove(cartProduct);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        //Clear Cart after order is placed
        public async Task<bool> ClearCartAsync(string userId)
        {
            var cart = await context.Carts
                .Include(c => c.CartProducts)
                .FirstOrDefaultAsync(c => c.CustomerId == userId);
            if (cart != null)
            {
                context.CartProducts.RemoveRange(cart.CartProducts);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // Add products to cart
        public async Task<bool> AddProductToCartAsync(int cartId, int productId, int quantity)
        {
            var cart = await context.Carts
                .Include(c => c.CartProducts)
                .FirstOrDefaultAsync(c => c.CartId == cartId);
            if (cart != null)
            {
                var existingProduct = cart.CartProducts
                    .FirstOrDefault(cp => cp.ProductId == productId);
                if (existingProduct != null)
                {
                    existingProduct.Quantity += quantity;
                }
                else
                {
                    context.CartProducts.Add(new CartProduct
                    {
                        CartId = cartId,
                        ProductId = productId,
                        Quantity = quantity
                    });
                }
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
