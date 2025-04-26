using DTech.Models.EF;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DTech.DAO
{
    public class RoleDAO(RoleManager<IdentityRole> roleManager)
    {

        // Return all content of table
        public async Task<List<IdentityRole>> GetListAsync()
        {
            return await roleManager.Roles.ToListAsync();
        }

        // Return one row of table
        public async Task<IdentityRole?> GetByIdAsync(string? id)
        {
            // Check if id is valid
            var role = await roleManager.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);
            if (role == null)
            {
                return null;
            }
            return role;
        }

        // Add new row to table
        public async Task<bool> AddAsync(IdentityRole role)
        {
            try
            {
                var result = await roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // Check if role existed
        public async Task<bool> CheckRoleAsync(string? roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return false;
            }

            // Use RoleManager's RoleExistsAsync method directly
            var roleExists = await roleManager.RoleExistsAsync(roleName);
            return roleExists;
        }

        // Return RoleId of Customer
        public async Task<string?> GetCusomerRoleId(string? roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return null;
            }

            // Use RoleManager's RoleExistsAsync method directly
            var role = await roleManager.FindByNameAsync(roleName);

            if (role != null)
            {
                return role.Id;
            }

            return null;
        }
    }
}
