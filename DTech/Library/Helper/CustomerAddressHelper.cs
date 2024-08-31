using DTech.Models.EF;
using System.Net;

namespace DTech.Library.Helper
{
    public class CustomerAddressHelper
    {
        private readonly EcommerceWebContext _context;

        public CustomerAddressHelper(EcommerceWebContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(CustomerAddress customerAddress)
        {
            if(customerAddress != null)
            {
                _context.CustomerAddresses.Add(customerAddress);
                await _context.SaveChangesAsync();
            }
            
        }

        public async Task DeleteAsync(List<CustomerAddress> customerAddress)
        {
            if (customerAddress != null && customerAddress.Any())
            {
                _context.CustomerAddresses.RemoveRange(customerAddress);
                await _context.SaveChangesAsync();
            }
            await Task.CompletedTask;
        }

        public async Task UpdateAsync(CustomerAddress address)
        {
            if (address != null)
            {
                _context.CustomerAddresses.Update(address);
                await _context.SaveChangesAsync();
            }
            await Task.CompletedTask;
        }

        // Method to retrieve an address by ID
        public async Task<CustomerAddress> GetAddressByIdAsync(int id)
        {
            return await _context.CustomerAddresses.FindAsync(id);
        }
    }
}
