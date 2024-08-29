using DTech.Models.EF;
using Microsoft.EntityFrameworkCore;

namespace DTech.Library.Helper
{
    public class AdminHelper
    {
        private readonly EcommerceWebContext _context;

        public AdminHelper(EcommerceWebContext context) { 
            _context = context;
        }

        public async Task<Admin> CheckExistAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
                .FirstOrDefaultAsync(m => m.AdminId == id);

            if (admin == null)
            {
                return NotFound();
            }

            return admin;
        }

        private Admin NotFound()
        {
            throw new NotImplementedException();
        }
    }
}
