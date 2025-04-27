using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DTech.Models.EF;
using DTech.Library;
using Newtonsoft.Json;
using DTech.DAO;
using System.Diagnostics;
using DTech.Library.Service;

namespace DTech.Areas.Admin.Controllers
{
    [Area("Admin")]
    [SetViewBagAttributes]
    public class AdvertisementsController(
        AdvertisementDAO advertisementDAO,
        CloudinaryService cloudinaryService
    ) : Controller
    {
        readonly string folderName = "Pre-thesis/Advertisement";

        // GET: Admin/Advertisements
        public async Task<IActionResult> Index()
        {
            return View(await advertisementDAO.GetListAsync());
        }

        // GET: Admin/Advertisements/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Advertisements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AdvertisementId,Name,Slug,Order,Image,CreatedBy,CreateDate,UpdatedBy,UpdateDate,Status,ImageUpload")] Advertisement advertisement)
        {
            if (ModelState.IsValid)
            {
                //Check if adv already exist
                advertisement.Slug = advertisement.Name?.ToLower().Replace(" ", "-");

                var slug = await advertisementDAO.CheckSlugAsync(advertisement.Slug);


                if (slug != null) {
                    TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Advertisement already exists!"));
                    return View(advertisement);
                }

                //Check if order already exist
                var order = await advertisementDAO.CheckOrderAsync(advertisement.Order);

                if (order)
                {
                    await advertisementDAO.UpdateOrderAsync(advertisement.Order);
                }

                //Image Upload
                string imageName;
                if (advertisement.ImageUpload != null && advertisement.ImageUpload.Length > 0)
                {
                    imageName = await cloudinaryService.UploadImageAsync(advertisement.ImageUpload, folderName);
                }
                else
                {
                    imageName = "noimg.png";
                }

                //Save to database
                advertisement.Status ??= 1;
                advertisement.Image = imageName;
                advertisement.CreateDate = DateTime.Now;
                advertisement.CreatedBy = "Admin1";
                
                await advertisementDAO.AddAsync(advertisement);

                //Success message
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Created successfully"));

                return RedirectToAction(nameof(Index));
            }
            TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Create fail, please check again!"));
            return View(advertisement);
        }

        // GET: Admin/Advertisements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var advertisement = await advertisementDAO.GetByIdAsync(id);
            if (advertisement == null)
            {
                return NotFound();
            }
            return View(advertisement);
        }

        // POST: Admin/Advertisements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AdvertisementId,Name,Slug,Order,Image,CreatedBy,CreateDate,UpdatedBy,UpdateDate,Status,ImageUpload")] Advertisement advertisement)
        {
            if (id != advertisement.AdvertisementId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Generate slug from the updated name
                    string newSlug = advertisement.Name?.ToLower().Replace(" ", "-") ?? string.Empty;

                    // Check if the slug is already used by another advertisement
                    var existingAdvertisement = await advertisementDAO.CheckSlugAsync(newSlug, advertisement.AdvertisementId);

                    if (existingAdvertisement != null)
                    {
                        TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Advertisement already exists!"));
                        return View(advertisement);
                    }

                    advertisement.Slug = newSlug;

                    //Update order
                    var originalAdvertisement = await advertisementDAO.GetByIdAsync(advertisement.AdvertisementId);
                    int? oldOrder = originalAdvertisement?.Order;
                    int? newOrder = advertisement.Order;

                    // Only update orders if they're different
                    if (oldOrder != newOrder)
                    {
                        await advertisementDAO.UpdateOrderAsync(oldOrder, newOrder, advertisement.AdvertisementId);
                    }

                    //Change Photo
                    if (advertisement.ImageUpload != null && advertisement.ImageUpload.Length > 0)
                    {
                        string imageName = await cloudinaryService.ChangeImageAsync(
                            oldfile: advertisement.Image ?? string.Empty,
                            newfile: advertisement.ImageUpload,
                            filepath: folderName
                        );
                        advertisement.Image = imageName;
                    }

                    advertisement.UpdateDate = DateTime.Now;
                    advertisement.UpdatedBy = "Admin1";

                    await advertisementDAO.UpdateAsync(advertisement);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await advertisementDAO.CheckIdAsync(advertisement.AdvertisementId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Edited successfully"));
                return RedirectToAction(nameof(Index));
            }
            TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Edit fail, please check again!"));
            return View(advertisement);
        }

        // GET: Admin/Advertisements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var advertisement = await advertisementDAO.GetByIdAsync(id);
            if (advertisement == null)
            {
                return NotFound();
            }

            return View(advertisement);
        }

        // POST: Admin/Advertisements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var advertisement = await advertisementDAO.GetByIdAsync(id);
            var deleteImageResult = false;
            var deleteAdvertisementResult = false;

            if (advertisement != null)
            {
                if (!string.IsNullOrEmpty(advertisement.Image))
                {
                    //Delete image
                    deleteImageResult = await cloudinaryService.DeleteImageAsync(advertisement.Image);
                }
                else
                {
                    deleteImageResult = true; // No image to delete, consider it successful
                }
                deleteAdvertisementResult = await advertisementDAO.DeleteAsync(id);
            }

            if (deleteImageResult && deleteAdvertisementResult)
            {
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Deleted successfully"));
            }
            else
            {
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Delete failed, please check again!"));
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> StatusChange(int id)
        {
            var advertisement = await advertisementDAO.GetByIdAsync(id);

            if (advertisement == null)
            {
                return RedirectToAction(nameof(Index));
            }

            advertisement.Status = (advertisement.Status == 1) ? 0 : 1;
            advertisement.UpdateDate = DateTime.Now;
            advertisement.UpdatedBy = "Admin1";

            await advertisementDAO.UpdateAsync(advertisement);

            TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Edited successfully"));
            return RedirectToAction(nameof(Index));
        }
    }
}
