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

        //Return lastest user
        public async Task<List<ApplicationUser>> GetLastestCustomerAsync()
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
                    filteredUsers.Add(user);
                }
            }

            return [.. filteredUsers.OrderByDescending(c => c.CreateDate).Take(8)];
        }

        //Return count users today
        public async Task<int> GetCountCustomerAsync()
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
                    filteredUsers.Add(user);
                }
            }
            var countUsersToday = filteredUsers.Count(c => c.CreateDate.HasValue && c.CreateDate.Value.Date == DateTime.Now.Date);
            return countUsersToday;
        }

        //Return one row of table
        public async Task<ApplicationUser?> GetByIdAsync(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            var customer = await userManager.FindByIdAsync(id);
            if (customer != null)
            {
                customer.CustomerAddresses = await context.CustomerAddresses
                    .Where(ca => ca.CustomerId == customer.Id)
                    .Include(a => a.Ward)
                    .Include(a => a.District)
                    .Include(a => a.Province)
                    .ToListAsync();
                customer.Orders = await context.Orders
                    .Include(o => o.Status)
                    .Where(o => o.CustomerId == customer.Id)
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
                var existingUser = await userManager.FindByIdAsync(customer.Id);
                if (existingUser == null) return false;

                // Update only the fields that are allowed
                existingUser.FullName = customer.FullName;
                existingUser.Email = customer.Email;
                existingUser.PhoneNumber = customer.PhoneNumber;
                existingUser.Gender = customer.Gender;
                existingUser.DateOfBirth = customer.DateOfBirth;
                existingUser.Image = customer.Image;
                existingUser.UpdatedBy = customer.UpdatedBy;
                existingUser.UpdateDate = customer.UpdateDate;

                var result = await userManager.UpdateAsync(existingUser);
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

        //Check if phone is exsting when editing
        public async Task<bool> CheckPhoneAsync(string? phone, string? Id)
        {
            return await context.Users.AnyAsync(e => e.PhoneNumber == phone && e.Id != Id);
        }

        //Check if email is exsting when editing
        public async Task<bool> CheckEmailAsync(string? email, string? Id)
        {
            return await context.Users.AnyAsync(e => e.Email == email && e.Id != Id);
        }

        //Get order by customer id
        public async Task<List<Order>> GetOrdersByCustomerIdAsync(string? customerId)
        {
            if (customerId == null)
            {
                return [];
            }
            return await context.Orders
                .Where(o => o.CustomerId == customerId)
                .Include(o => o.OrderProducts)
                .Include(o => o.Status)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        //Get coupon by customer id
        public async Task<List<CustomerCoupon>> GetCouponsByCustomerIdAsync(string? customerId)
        {
            if (customerId == null)
            {
                return [];
            }
            return await context.CustomerCoupons
                .Where(c => c.CustomerId == customerId)
                .Include(c => c.Coupon)
                .Where(predicate => predicate.Coupon!.Status == 1)
                .ToListAsync();
        }

        // Get address by customer id
        public async Task<List<CustomerAddress>> GetAddressesByCustomerIdAsync(string? customerId)
        {
            if (customerId == null)
            {
                return [];
            }
            return await context.CustomerAddresses
                .Where(c => c.CustomerId == customerId)
                .Include(a => a.Ward)
                .Include(a => a.District)
                .Include(a => a.Province)
                .ToListAsync();
        }

        // Updated Password
        public async Task<bool> ChangePasswordAsync(string id, string? oldPassword, string? newPassword)
        {
            if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
            {
                return false;
            }

            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return false;
            }

            var result = await userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            return result.Succeeded;
        }
    }
}