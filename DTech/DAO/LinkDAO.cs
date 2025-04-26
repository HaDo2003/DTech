using DTech.Models.EF;
using Microsoft.EntityFrameworkCore;

namespace DTech.DAO
{
    public class LinkDAO(
        EcommerceWebContext context
    ){
        //Return one row of table
        public async Task<Link?> GetByIdAsync(int? id, string typeLink)
        {
            if (id == null)
            {
                return null;
            }

            var link = await context.Links.AsNoTracking().FirstOrDefaultAsync(a => a.LinkId == id && a.TypeLink == typeLink);
            return link;
        }
        //Add new row to table
        public async Task<bool> AddAsync(Link link)
        {
            try
            {
                context.Links.Add(link);
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
        public async Task<bool> UpdateAsync(Link link)
        {
            try
            {
                context.Links.Update(link);
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
        public async Task<bool> DeleteAsync(Link link)
        {
            try
            {
                context.Links.Remove(link);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
