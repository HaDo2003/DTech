using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DTech.Models.EF;
using DTech.Library;
using Newtonsoft.Json;
using DTech.DAO;
using DTech.Library.Service;
using Microsoft.AspNetCore.Authorization;

namespace DTech.Areas.Admin.Controllers
{
    [Area("Admin")]
    [SetViewBagAttributes]
    [Authorize(Roles = "Admin,Seller")]
    public class BrandsController(
        BrandDAO brandDAO,
        CloudinaryService cloudinaryService
    ) : Controller
    {
        readonly string folderName = "Pre-thesis/Brand";

        // GET: Admin/Brands
        public async Task<IActionResult> Index()
        {
            return View(await brandDAO.GetListAsync());
        }
       
        // GET: Admin/Brands/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Brands/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BrandId,Name,Slug,Logo,CreatedBy,CreateDate,UpdatedBy,UpdateDate,Status,LogoUpload")] Brand brand)
        {
            if (ModelState.IsValid)
            {
                //Check if brand already exist
                brand.Slug = brand.Name?.ToLower().Replace(" ", "-");

                var slug = await brandDAO.CheckSlugAsync(brand.Slug);

                if (slug != null)
                {
                    TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Brand already exists!"));
                    return View(brand);
                }

                //Logo Upload
                string imageName;
                if (brand.LogoUpload != null && brand.LogoUpload.Length > 0)
                {
                    imageName = await cloudinaryService.UploadImageAsync(brand.LogoUpload, folderName);
                }
                else
                {
                    imageName = "noimg.png";
                }

                //Save to database
                brand.Logo = imageName;
                brand.CreateDate = DateTime.Now;
                brand.CreatedBy = "Admin1";

                await brandDAO.AddAsync(brand);

                //Success message
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Created successfully"));
                return RedirectToAction(nameof(Index));
            }
            TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Create fail, please check again!"));
            return View(brand);
        }

        // GET: Admin/Brands/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await brandDAO.GetByIdAsync(id);
            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }

        // POST: Admin/Brands/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BrandId,Name,Slug,Logo,CreatedBy,CreateDate,UpdatedBy,UpdateDate,Status,LogoUpload")] Brand brand)
        {
            if (id != brand.BrandId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Generate slug from the updated name
                    string newSlug = brand.Name?.ToLower().Replace(" ", "-") ?? string.Empty;

                    // Check if the slug is already used by another brand
                    var existingBrand = await brandDAO.CheckSlugAsync(newSlug, brand.BrandId);

                    if (existingBrand != null)
                    {
                        TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Brand already exists!"));
                        return View(brand);
                    }

                    brand.Slug = newSlug;

                    //Change Photo
                    if (brand.LogoUpload != null && brand.LogoUpload.Length > 0)
                    {
                        string imageName = await cloudinaryService.ChangeImageAsync(
                            brand.Logo ?? string.Empty, 
                            brand.LogoUpload, 
                            folderName);
                        brand.Logo = imageName;
                    }
                    
                    brand.UpdateDate = DateTime.Now;
                    brand.UpdatedBy = "Admin1";

                    await brandDAO.UpdateAsync(brand);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (! await brandDAO.CheckIdAsync(brand.BrandId))
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
            return View(brand);
        }

        // GET: Admin/Brands/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await brandDAO.GetByIdAsync(id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        // POST: Admin/Brands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var brand = await brandDAO.GetByIdAsync(id);
            var deleteImageResult = false;
            var deleteBrandResult = false;
            if (brand != null)
            {
                if (!string.IsNullOrEmpty(brand.Logo))
                {
                    //Delete image
                    deleteImageResult = await cloudinaryService.DeleteImageAsync(brand.Logo);
                }
                else
                {
                    deleteImageResult = true; // No image to delete, consider it successful
                }
                deleteBrandResult = await brandDAO.DeleteAsync(brand);
            }

            if (deleteImageResult && deleteBrandResult)
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
            var brand = await brandDAO.GetByIdAsync(id);

            if (brand == null)
            {
                return RedirectToAction(nameof(Index));
            }

            brand.Status = (brand.Status == 1) ? 0 : 1;
            brand.UpdateDate = DateTime.Now;
            brand.UpdatedBy = "Admin1";

            await brandDAO.UpdateAsync(brand);

            TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Edited successfully"));
            return RedirectToAction(nameof(Index));
        }
    }
}