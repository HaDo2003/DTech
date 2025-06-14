using DTech.Models.EF;
using Microsoft.EntityFrameworkCore;

namespace DTech.DAO
{
    public class OrderDAO(
        EcommerceWebContext context
    )
    {
        //Return all content of table
        public async Task<List<Order>> GetListAsync()
        {
            return await context.Orders
                .AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.Payment)
                .Include(o => o.Shipping)
                .Include(o => o.Status)
                .ToListAsync();
        }

        //Return all content of table by descending id
        public async Task<List<Order>> GetListByDecendingAsync()
        {
            return await context.Orders
                .AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.Payment)
                .Include(o => o.Shipping)
                .Include(o => o.Status)
                .Include(o => o.OrderProducts)
                .OrderByDescending(o => o.OrderId)
                .Take(6)
                .ToListAsync();
        }

        //Return one row of table
        public async Task<Order?> GetByIdAsync(int? id)
        {
            if (id == null)
            {
                return null;
            }

            var order = await context.Orders
                .AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.Payment)
                .ThenInclude(p => p!.PaymentMethod)
                .Include(o => o.Shipping)
                .Include(o => o.Status)
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product!)
                .Include(o => o.Province)
                .Include(o => o.District)
                .Include(o => o.Ward)
                .FirstOrDefaultAsync(a => a.OrderId == id);
            return order;
        }

        //Add new row to table
        public async Task<bool> AddAsync(Order order)
        {
            try
            {
                context.Orders.Add(order);
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
        public async Task<bool> UpdateAsync(Order order)
        {
            try
            {
                context.Orders.Update(order);
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
        public async Task<bool> DeleteAsync(Order order)
        {
            try
            {
                context.Orders.Remove(order);
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
            return await context.Orders.AnyAsync(e => e.OrderId == id);
        }

        //Add Order Detail to Order
        public async Task<bool> AddOrderDetailAsync(List<OrderProduct> orderProduct)
        {
            try
            {
                context.OrderProducts.AddRange(orderProduct);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        //Get order by paymentId
        public async Task<Order?> GetByPaymentIdAsync(int? paymentId)
        {
            if (paymentId == null)
            {
                return null;
            }
            return await context.Orders
                .AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.Payment)
                .Include(o => o.Shipping)
                .Include(o => o.Status)
                .FirstOrDefaultAsync(o => o.PaymentId == paymentId);
        }
    }
}
