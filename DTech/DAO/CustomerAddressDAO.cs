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


        public async Task<bool> CreateAsync(CustomerAddress customerAddress)
        {
            if (customerAddress != null)
            {
                context.CustomerAddresses.Add(customerAddress);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        //Delete all addresses of a customer
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

        //Delete one address by id
        public async Task<bool> DeleteByIdAsync(int? id)
        {
            var customerAddress = await context.CustomerAddresses.FindAsync(id);
            if (customerAddress != null)
            {
                try
                {
                    context.CustomerAddresses.Remove(customerAddress);
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

        // Method to update an address
        public async Task<bool> UpdateAsync(CustomerAddress address)
        {
            if (address != null)
            {
                context.CustomerAddresses.Update(address);
                await context.SaveChangesAsync();
                return true;
            }
            await Task.CompletedTask;
            return false;
        }

        // Method to retrieve an address by ID
        public async Task<CustomerAddress> GetAddressByIdAsync(int id)
        {
            return await context.CustomerAddresses.FindAsync(id);
        }

        // Method to retrieve all addresses of a customer
        public async Task<bool> CustomerAddressExists(int id)
        {
            return await context.CustomerAddresses.AnyAsync(e => e.AddressId == id);
        }

        // Method to check if an address exsists
        public async Task<bool> AddressExists(int? id)
        {
            return await context.CustomerAddresses.AnyAsync(e => e.AddressId == id);
        }
    }
}
