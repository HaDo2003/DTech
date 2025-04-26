using DTech.Models.EF;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DTech.DAO
{
    public class CustomerDAO
    (
        EcommerceWebContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
    )
    {
        //Return all content of table
        public async Task<List<ApplicationUser>> GetListAsync()
        {
            var users = await userManager.Users.AsNoTracking().ToListAsync();
            var roles = await roleManager.Roles.AsNoTracking().ToListAsync();

            var filteredUsers = new List<ApplicationUser>();

            foreach (var user in users)
            {
                var role = roles.FirstOrDefault(r => r.Id == user.RoleId);
                var roleName = role?.Name ?? string.Empty;

                if (roleName == "Customer")
                {
                    user.RoleName = roleName ?? string.Empty;
                    user.CustomerAddresses = await context.CustomerAddresses
                        .Where(ca => ca.CustomerId == user.Id)
                        .ToListAsync();
                    filteredUsers.Add(user);
                }
            }

            return filteredUsers;
            
        }

        //Return one row of table
        public async Task<ApplicationUser?> GetByIdAsync(string? id)
        {
            if (id == null)
            {
                return null;
            }

            var customer = await userManager.FindByIdAsync(id);
            if (customer != null)
            {
                customer.CustomerAddresses = await context.CustomerAddresses
                    .Where(ca => ca.CustomerId == customer.Id)
                    .ToListAsync();
            }

            return customer;
        }

        //Add new row to table
        public async Task<bool> AddAsync(ApplicationUser customer)
        {
            try
            {
                var result = await userManager.CreateAsync(customer, customer.PasswordHash ?? string.Empty);
                if (result.Succeeded)
                {
                    var role = await roleManager.FindByIdAsync(customer.RoleId);
                    if (role != null)
                    {
                        // Assign the role to the user
                        await userManager.AddToRoleAsync(customer, role.Name ?? string.Empty);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        //Update row in table
        public async Task<bool> UpdateAsync(ApplicationUser customer)
        {
            try
            {
                var result = await userManager.UpdateAsync(customer);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        // DeleteAsync
        public async Task<bool> DeleteAsync(string id)
        {
            var customer = await GetByIdAsync(id);
            if (customer == null)
            {
                return false;
            }

            try
            {
                var result = await userManager.DeleteAsync(customer);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        //Check if id is valid
        public async Task<bool> CheckIdAsync(string? id)
        {
            return await context.Users.AnyAsync(e => e.Id == id);
        }

        //Check if account is exsting
        public async Task<bool> CheckAccountAsync(string? account)
        {
            return await context.Users.AnyAsync(e => e.UserName == account);
        }

        //Check if phone is exsting
        public async Task<bool> CheckPhoneAsync(string? phone)
        {
            return await context.Users.AnyAsync(e => e.PhoneNumber == phone);
        }

        //Check if email is exsting
        public async Task<bool> CheckEmailAsync(string? email)
        {
            return await context.Users.AnyAsync(e => e.Email == email);
        }
    }
}