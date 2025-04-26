using DTech.Models.EF;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace DTech.DAO
{
    public class CustomerAddressDAO(
        EcommerceWebContext context
    )
    {
        //Return one row of table
        public async Task<CustomerAddress?> GetByIdAsync(int? id)
        {
            if (id == null)
            {
                return null;
            }

            var CustomerAddress = await context.CustomerAddresses
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AddressId == id);
            return CustomerAddress;
        }


        public async Task CreateAsync(CustomerAddress customerAddress)
        {
            if (customerAddress != null)
            {
                context.CustomerAddresses.Add(customerAddress);
                await context.SaveChangesAsync();
            }

        }

        public async Task<bool> DeleteAsync(string id)
        {
            var customerAddress = await context.CustomerAddresses
                            .Where(c => c.CustomerId == id)
                            .ToListAsync();
            if (customerAddress != null && customerAddress.Count != 0)
            {
                try
                {
                    context.CustomerAddresses.RemoveRange(customerAddress);
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
                //customerAddress not found
                return false;
            }
        }

        public async Task UpdateAsync(CustomerAddress address)
        {
            if (address != null)
            {
                context.CustomerAddresses.Update(address);
                await context.SaveChangesAsync();
            }
            await Task.CompletedTask;
        }

        // Method to retrieve an address by ID
        public async Task<CustomerAddress> GetAddressByIdAsync(int id)
        {
            return await context.CustomerAddresses.FindAsync(id);
        }

        public async Task<bool> CustomerAddressExists(int id)
        {
            return await context.CustomerAddresses.AnyAsync(e => e.AddressId == id);
        }
    }
}
