using DTech.Models.EF;
using Microsoft.EntityFrameworkCore;


namespace DTech.DAO
{
    public class OrderStatusDAO(
        EcommerceWebContext context
    ){
        //Return all content of table
        public async Task<List<OrderStatus>> GetListAsync()
        {
            return await context.OrderStatuses.AsNoTracking().ToListAsync();
        }
        //Return one row of table
        public async Task<OrderStatus?> GetByIdAsync(int? id)
        {
            if (id == null)
            {
                return null;
            }
            var orderStatus = await context.OrderStatuses.AsNoTracking().FirstOrDefaultAsync(a => a.StatusId == id);
            return orderStatus;
        }
        //Add new row to table
        public async Task<bool> AddAsync(OrderStatus orderStatus)
        {
            try
            {
                context.OrderStatuses.Add(orderStatus);
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
        public async Task<bool> UpdateAsync(OrderStatus orderStatus)
        {
            try
            {
                context.OrderStatuses.Update(orderStatus);
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
        public async Task<bool> DeleteAsync(OrderStatus orderStatus)
        {
            try
            {
                context.OrderStatuses.Remove(orderStatus);
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
