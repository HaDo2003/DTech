using DTech.Models.EF;
using Microsoft.EntityFrameworkCore;

namespace DTech.DAO
{
    public class FeedbackDAO
    (
        EcommerceWebContext context
    ){
        //Return all content of table
        public async Task<List<Feedback>> GetListAsync()
        {
            return await context.Feedbacks.AsNoTracking().ToListAsync();
        }

        //Return one row of table
        public async Task<Feedback?> GetByIdAsync(int? id)
        {
            if (id == null)
            {
                return null;
            }

            var advertisement = await context.Feedbacks.AsNoTracking().FirstOrDefaultAsync(a => a.FeedbackId == id);
            return advertisement;
        }

        //Delete row in table
        public async Task<bool> DeleteAsync(Feedback advertisement)
        {
            try
            {
                context.Feedbacks.Remove(advertisement);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        //Add new row to table
        public async Task<bool> AddAsync(Feedback feedback)
        {
            try
            {
                await context.Feedbacks.AddAsync(feedback);
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
