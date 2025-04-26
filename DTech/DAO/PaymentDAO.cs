using DTech.Models.EF;
using Microsoft.EntityFrameworkCore;

namespace DTech.DAO
{
    public class PaymentDAO
    (
        EcommerceWebContext context
    ){
        //Return all content of table
        public async Task<List<Payment>> GetListAsync()
        {
            return await context.Payments
                .AsNoTracking()
                .Include(p => p.PaymentMethod)
                .ToListAsync();
        }

        //Return one row of table
        public async Task<Payment?> GetByIdAsync(int? id)
        {
            if (id == null)
            {
                return null;
            }

            var payment = await context.Payments
                .AsNoTracking()
                .Include(p => p.PaymentMethod)
                .FirstOrDefaultAsync(a => a.PaymentId == id);
            return payment;
        }

        //Add new row to table
        public async Task<bool> AddAsync(Payment payment)
        {
            try
            {
                context.Payments.Add(payment);
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
        public async Task<bool> UpdateAsync(Payment payment)
        {
            try
            {
                context.Payments.Update(payment);
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
        public async Task<bool> DeleteAsync(Payment payment)
        {
            try
            {
                context.Payments.Remove(payment);
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
            return await context.Payments.AnyAsync(e => e.PaymentId == id);
        }
    }
}
