using DTech.Models.EF;
using Microsoft.EntityFrameworkCore;

namespace DTech.DAO
{
    public class SpecificationDAO(
        EcommerceWebContext context
    )
    {
        //Get Specifications by id
        public async Task<Specification?> GetSpecificationsByIdAsync(int? id)
        {
            if (id == null)
            {
                return null;
            }
            var specifications = await context.Specifications.FindAsync(id);
            return specifications;
        }

        //Remove specifications by id
        public async Task<bool> RemoveSpecificationsByIdAsync(int id)
        {
            try
            {
                var specifications = await GetSpecificationsByIdAsync(id);
                if (specifications != null)
                {
                    context.Specifications.Remove(specifications);
                    await context.SaveChangesAsync();
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
    }
}
