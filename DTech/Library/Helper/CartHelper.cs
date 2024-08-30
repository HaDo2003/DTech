using DTech.Models.EF;
using Microsoft.AspNetCore.Mvc;

namespace DTech.Library.Helper
{
    public class CartHelper
    {
        private readonly EcommerceWebContext _context;

        public CartHelper(EcommerceWebContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Cart cart)
        {
            _context.Add(cart);
            await _context.SaveChangesAsync();

            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Cart cart)
        {
            if (cart != null)
            {
                _context.Remove(cart);
            }
            await Task.CompletedTask;
        }
    }
}
