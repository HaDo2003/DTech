using DTech.Models.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace DTech.DAO
{
    public class AdminDAO(
        UserManager<ApplicationUser> userManager, 
        RoleManager<IdentityRole> roleManager
    )
    {
        public async Task<List<ApplicationUser>> GetListAsync()
        {
            var users = await userManager.Users.AsNoTracking().ToListAsync();
            var roles = await roleManager.Roles.AsNoTracking().ToListAsync();

            var filteredUsers = new List<ApplicationUser>();

            foreach (var user in users)
            {
                var role = roles.FirstOrDefault(r => r.Id == user.RoleId);
                var roleName = role?.Name ?? string.Empty;

                if (roleName == "Admin" || roleName == "Seller")
                {
                    user.RoleName = roleName ?? string.Empty;
                    filteredUsers.Add(user);
                }
            }

            return filteredUsers;
        }

        public async Task<ApplicationUser?> GetByIdAsync(string? id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            var user = await userManager.FindByIdAsync(id);

            if (user != null)
            {
                // Get the user's role name
                var roles = await userManager.GetRolesAsync(user);
                var roleName = roles.FirstOrDefault();

                if (!string.IsNullOrEmpty(roleName))
                {
                    var role = await roleManager.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
                    if (role != null)
                    {
                        user.RoleId = role.Id;
                        user.RoleName = role.Name;
                    }
                }
            }

            return user;
        }

        public async Task<bool> AddAsync(ApplicationUser admin)
        {
            try
            {
                var result = await userManager.CreateAsync(admin, admin.PasswordHash ?? string.Empty);
                if (result.Succeeded)
                {
                    var role = await roleManager.FindByIdAsync(admin.RoleId);
                    if (role != null)
                    {
                        // Assign the role to the user
                        await userManager.AddToRoleAsync(admin, role.Name ?? string.Empty);
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

        public async Task<bool> UpdateAsync(ApplicationUser admin, string newRoleId)
        {
            try
            {
                var existingUser = await userManager.FindByIdAsync(admin.Id);
                if (existingUser == null) return false;

                // Update fields
                existingUser.Email = admin.Email;
                existingUser.PhoneNumber = admin.PhoneNumber;
                existingUser.UserName = admin.UserName;
                existingUser.Gender = admin.Gender;
                existingUser.DateOfBirth = admin.DateOfBirth;
                existingUser.Image = admin.Image;
                existingUser.Address = admin.Address;
                existingUser.UpdatedBy = admin.UpdatedBy;
                existingUser.UpdateDate = DateTime.Now;

                // Update user info
                var updateResult = await userManager.UpdateAsync(existingUser);
                if (!updateResult.Succeeded) return false;

                // Change role if necessary
                var currentRoles = await userManager.GetRolesAsync(existingUser);
                if (currentRoles.Any())
                {
                    await userManager.RemoveFromRolesAsync(existingUser, currentRoles);
                }

                var newRole = await roleManager.FindByIdAsync(newRoleId);
                if (newRole != null)
                {
                    await userManager.AddToRoleAsync(existingUser, newRole.Name ?? string.Empty);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(ApplicationUser admin)
        {
            try
            {
                var result = await userManager.DeleteAsync(admin);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> CheckIdAsync(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }
            var user = await userManager.FindByIdAsync(id);
            return user != null;
        }

        public async Task<bool> CheckAccountAsync(string? email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }
            var user = await userManager.FindByEmailAsync(email);
            return user != null;
        }
    }
}
