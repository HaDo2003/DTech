using DTech.Models.EF;
using Microsoft.EntityFrameworkCore;

namespace DTech.DAO
{
    public class CouponDAO(
        EcommerceWebContext context
    ){
        //Return all content of table
        public async Task<List<Coupon>> GetListAsync()
        {
            return await context.Coupons.AsNoTracking().ToListAsync();
        }

        //Return one row of table
        public async Task<Coupon?> GetByIdAsync(int? id)
        {
            if (id == null)
            {
                return null;
            }

            var coupon = await context.Coupons.AsNoTracking().FirstOrDefaultAsync(a => a.CouponId == id);
            return coupon;
        }

        //Add new row to table
        public async Task<bool> AddAsync(Coupon coupon)
        {
            try
            {
                context.Coupons.Add(coupon);
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
        public async Task<bool> UpdateAsync(Coupon coupon)
        {
            try
            {
                context.Coupons.Update(coupon);
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
        public async Task<bool> DeleteAsync(Coupon coupon)
        {
            try
            {
                context.Coupons.Remove(coupon);
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
            return await context.Coupons.AnyAsync(e => e.CouponId == id);
        }

        // Check Slug
        public async Task<Coupon?> CheckSlugAsync(string? slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return null;
            }

            var coupon = await context.Coupons.FirstOrDefaultAsync(a => a.Slug == slug);
            return coupon;
        }

        // Check if the slug is already used by another coupon
        public async Task<Coupon?> CheckSlugAsync(string? newSlug, int? CouponId)
        {
            if (string.IsNullOrEmpty(newSlug))
            {
                return null;
            }

            var coupon = await context.Coupons
                .FirstOrDefaultAsync(a => a.Slug == newSlug && a.CouponId != CouponId);
            return coupon;
        }
    }
}
