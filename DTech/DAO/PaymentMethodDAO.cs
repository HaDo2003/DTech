using DTech.Models.EF;
using Microsoft.EntityFrameworkCore;

namespace DTech.DAO
{
    public class PaymentMethodDAO
    (
        EcommerceWebContext context
    ){
        //Return all content of table
        public async Task<List<PaymentMethod>> GetListAsync()
        {
            return await context.PaymentMethods.AsNoTracking().ToListAsync();
        }

        //Return one row of table
        public async Task<PaymentMethod?> GetByIdAsync(int? id)
        {
            if (id == null)
            {
                return null;
            }

            var paymentMethod = await context.PaymentMethods.AsNoTracking().FirstOrDefaultAsync(a => a.PaymentMethodId == id);
            return paymentMethod;
        }

        //Add new row to table
        public async Task<bool> AddAsync(PaymentMethod paymentMethod)
        {
            try
            {
                context.PaymentMethods.Add(paymentMethod);
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
        public async Task<bool> UpdateAsync(PaymentMethod paymentMethod)
        {
            try
            {
                context.PaymentMethods.Update(paymentMethod);
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
        public async Task<bool> DeleteAsync(PaymentMethod paymentMethod)
        {
            try
            {
                context.PaymentMethods.Remove(paymentMethod);
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
            return await context.PaymentMethods.AnyAsync(e => e.PaymentMethodId == id);
        }

        //Check if payment method is existing
        public async Task<bool> CheckDescritionAsync(string? des)
        {
            return await context.PaymentMethods.AnyAsync(e => e.Description == des);
        }

        public async Task<PaymentMethod?> CheckDescritionAsync(string? des, int id)
        {
            if (string.IsNullOrEmpty(des))
            {
                return null;
            }
            var paymentMethod = await context.PaymentMethods
                .FirstOrDefaultAsync(e => e.Description == des && e.PaymentMethodId != id);
            return paymentMethod;
        }
    }
}
