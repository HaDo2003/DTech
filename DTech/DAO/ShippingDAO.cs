using DTech.Models.EF;
using Microsoft.EntityFrameworkCore;

namespace DTech.DAO
{
    public class ShippingDAO(
        EcommerceWebContext context
    ){
        //Return all content of table
        public async Task<List<Shipping>> GetListAsync()
        {
            return await context.Shippings.AsNoTracking().ToListAsync();
        }
        //Return one row of table
        public async Task<Shipping?> GetByIdAsync(int? id)
        {
            if (id == null)
            {
                return null;
            }
            var shipping = await context.Shippings.AsNoTracking().FirstOrDefaultAsync(a => a.ShippingId == id);
            return shipping;
        }
        //Add new row to table
        public async Task<bool> AddAsync(Shipping shipping)
        {
            try
            {
                context.Shippings.Add(shipping);
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
        public async Task<bool> UpdateAsync(Shipping shipping)
        {
            try
            {
                context.Shippings.Update(shipping);
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
        public async Task<bool> DeleteAsync(Shipping shipping)
        {
            try
            {
                context.Shippings.Remove(shipping);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
