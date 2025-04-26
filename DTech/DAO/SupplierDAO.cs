using DTech.Models.EF;
using Microsoft.EntityFrameworkCore;

namespace DTech.DAO
{
    public class SupplierDAO
    (
        EcommerceWebContext context
    ){
        //Return all content of table
        public async Task<List<Supplier>> GetListAsync()
        {
            return await context.Suppliers.AsNoTracking().ToListAsync();
        }

        //Return one row of table
        public async Task<Supplier?> GetByIdAsync(int? id)
        {
            if (id == null)
            {
                return null;
            }

            var supplier = await context.Suppliers.AsNoTracking().FirstOrDefaultAsync(a => a.SupplierId == id);
            return supplier;
        }

        //Add new row to table
        public async Task<bool> AddAsync(Supplier supplier)
        {
            try
            {
                context.Suppliers.Add(supplier);
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
        public async Task<bool> UpdateAsync(Supplier supplier)
        {
            try
            {
                context.Suppliers.Update(supplier);
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
        public async Task<bool> DeleteAsync(Supplier supplier)
        {
            try
            {
                context.Suppliers.Remove(supplier);
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
            return await context.Suppliers.AnyAsync(e => e.SupplierId == id);
        }

        // Check Slug
        public async Task<Supplier?> CheckSlugAsync(string? slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return null;
            }

            var supplier = await context.Suppliers.FirstOrDefaultAsync(a => a.Slug == slug);
            return supplier;
        }

        // Check if the slug is already used by another supplier
        public async Task<Supplier?> CheckSlugAsync(string? newSlug, int? SupplierId)
        {
            if (string.IsNullOrEmpty(newSlug))
            {
                return null;
            }

            var supplier = await context.Suppliers
                .FirstOrDefaultAsync(a => a.Slug == newSlug && a.SupplierId != SupplierId);
            return supplier;
        }
    }
}
