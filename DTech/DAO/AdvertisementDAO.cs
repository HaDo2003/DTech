using DTech.Models.EF;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace DTech.DAO
{
    public class AdvertisementDAO(
        EcommerceWebContext context
    ){
        //Return all content of table
        public async Task<List<Advertisement>> GetListAsync()
        {
            return await context.Advertisements
                .AsNoTracking()
                .OrderBy(a => a.Order)
                .ToListAsync();
        }

        //Return all content of table based on order
        public async Task<List<Advertisement>> GetOrderedListAsync()
        {
            return await context.Advertisements
                .AsNoTracking()
                .Where(a => a.Status == 1)
                .OrderBy(a => a.Order)
                .ToListAsync();
        }

        //Return one row of table
        public async Task<Advertisement?> GetByIdAsync(int? id)
        {
            if (id == null)
            {
                return null;
            }

            var advertisement = await context.Advertisements.AsNoTracking().FirstOrDefaultAsync(a => a.AdvertisementId == id);
            return advertisement;
        }

        //Add new row to table
        public async Task<bool> AddAsync(Advertisement advertisement)
        {
            try
            {
                context.Advertisements.Add(advertisement);
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
        public async Task<bool> UpdateAsync(Advertisement advertisement)
        {
            try
            {
                context.Advertisements.Update(advertisement);
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
        public async Task<bool> DeleteAsync(int? advId)
        {
            try
            {
                // Get the advertisement to be deleted
                var advertisementToDelete = await GetByIdAsync(advId);

                if (advertisementToDelete == null)
                    return false;

                // Get the order of the advertisement being deleted
                int? deletedOrder = advertisementToDelete.Order;

                // Remove the advertisement using BulkDelete
                await context.BulkDeleteAsync(new List<Advertisement> { advertisementToDelete });

                // Get all advertisements with order greater than the deleted one
                var advertisementsToUpdate = await context.Advertisements
                    .Where(a => a.Order > deletedOrder)
                    .OrderBy(a => a.Order)
                    .ToListAsync();

                // Shift down the order of all affected advertisements by 1
                foreach (var adv in advertisementsToUpdate)
                {
                    adv.Order -= 1;
                }

                // Perform a bulk update for the advertisements
                if (advertisementsToUpdate.Count != 0)
                    await context.BulkUpdateAsync(advertisementsToUpdate);

                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        //Check if id is valid
        public async Task<bool> CheckIdAsync(int? id)
        {
            return await context.Advertisements.AnyAsync(e => e.AdvertisementId == id);
        }

        // Check Slug
        public async Task<Advertisement?> CheckSlugAsync(string? slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return null;
            }

            var advertisement = await context.Advertisements.FirstOrDefaultAsync(a => a.Slug == slug);
            return advertisement; 
        }

        // Check if the slug is already used by another advertisement
        public async Task<Advertisement?> CheckSlugAsync(string? newSlug, int? AdvertisementId)
        {
            if (string.IsNullOrEmpty(newSlug))
            {
                return null;
            }

            var advertisement = await context.Advertisements
                .FirstOrDefaultAsync(a => a.Slug == newSlug && a.AdvertisementId != AdvertisementId);
            return advertisement;
        }

        //Check if order is exist
        public async Task<bool> CheckOrderAsync(int? order)
        {
            return await context.Advertisements.AnyAsync(e => e.Order == order);
        }

        //Update order when create new
        public async Task<bool> UpdateOrderAsync(int? order)
        {
            try
            {
                // Get all advertisements that need their Order updated
                var advertisementsToUpdate = await context.Advertisements
                    .Where(a => a.Order >= order)
                    .OrderBy(a => a.Order)
                    .ToListAsync();

                // Update the Order for each advertisement
                foreach (var advertisement in advertisementsToUpdate)
                {
                    advertisement.Order += 1;
                }
                await context.BulkUpdateAsync(advertisementsToUpdate);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        //Update order when edit
        public async Task<bool> UpdateOrderAsync(int? oldOrder, int? newOrder, int advId)
        {
            try
            {
                // If old order and new order are the same, no need to update
                if (oldOrder == newOrder) return true;

                // Get the max order in the database to prevent exceeding list length
                int maxOrder = await context.Advertisements.MaxAsync(a => (int?)a.Order) ?? 0;

                // Ensure new order doesn't exceed max order
                if (newOrder > maxOrder) newOrder = maxOrder;

                // Create a list of advertisements to update
                List<Advertisement> advertisementsToUpdate = [];

                if (oldOrder < newOrder)
                {
                    advertisementsToUpdate = await context.Advertisements
                        .Where(a => a.Order > oldOrder && a.Order <= newOrder && a.AdvertisementId != advId)
                        .ToListAsync();
                    
                    // Decrease the order for each item in the list
                    advertisementsToUpdate.ForEach(a => a.Order -= 1);
                }
                else if (oldOrder > newOrder)
                {
                    advertisementsToUpdate = await context.Advertisements
                        .Where(a => a.Order >= newOrder && a.Order < oldOrder && a.AdvertisementId != advId)
                        .ToListAsync();

                    // Increase the order for each item in the list
                    advertisementsToUpdate.ForEach(a => a.Order += 1);
                }

                // Bulk update the order of affected advertisements
                if (advertisementsToUpdate.Any())
                {
                    await context.BulkUpdateAsync(advertisementsToUpdate);
                }

                // Now, update the target advertisement with the new order
                var advertisementToUpdate = await context.Advertisements
                    .FirstOrDefaultAsync(a => a.AdvertisementId == advId);

                if (advertisementToUpdate != null)
                {
                    advertisementToUpdate.Order = newOrder;
                    await context.BulkUpdateAsync([advertisementToUpdate]);
                }

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