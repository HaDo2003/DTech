using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DTech.Models.EF;
using DTech.Library;
using Newtonsoft.Json;

namespace DTech.Areas.Admin.Controllers
{
    [Area("Admin")]
    [SetViewBagAttributes]
    public class AdvertisementsController : Controller
    {
        private readonly EcommerceWebContext _context;
        private readonly ImageSetter _settingImg;

        public AdvertisementsController(EcommerceWebContext context, ImageSetter settingImg)
        {
            _context = context;
            _settingImg = settingImg;
        }

        // GET: Admin/Advertisements
        public async Task<IActionResult> Index()
        {
            return View(await _context.Advertisements.ToListAsync());
        }

        // GET: Admin/Advertisements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var advertisement = await _context.Advertisements
                .FirstOrDefaultAsync(m => m.AdvId == id);
            if (advertisement == null)
            {
                return NotFound();
            }

            ViewBag.Status = new Dictionary<int, string>
            {
                { 1, "Available" },
                { 0, "Unavailable" },
            };
            return View(advertisement);
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
        public async Task<IActionResult> Create([Bind("AdvId,Name,Slug,Image,CreatedBy,CreateDate,UpdatedBy,UpdateDate,Status,ImageUpload")] Advertisement advertisement)
        {
            if (ModelState.IsValid)
            {
                //Check if adv already exist
                advertisement.Slug = advertisement.Name.ToLower().Replace(" ", "-");
                
                var slug = await _context.Advertisements
                    .FirstOrDefaultAsync(a => a.Slug == advertisement.Slug);

                if (slug != null) {
                    TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Advertisement already exists!"));
                    return View(advertisement);
                }

                //Image Upload
                string imageName = await _settingImg.UploadImageAsync(advertisement.ImageUpload, "img/AdvImg");

                //Save to database
                if (advertisement.Status == null)
                {
                    advertisement.Status = 1;
                }
                advertisement.Image = imageName;
                advertisement.CreateDate = DateTime.Now;
                advertisement.CreatedBy = "Admin1";

                _context.Add(advertisement);
                await _context.SaveChangesAsync();

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

            var advertisement = await _context.Advertisements.FindAsync(id);
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
        public async Task<IActionResult> Edit(int id, [Bind("AdvId,Name,Slug,Image,CreatedBy,CreateDate,UpdatedBy,UpdateDate,Status,ImageUpload")] Advertisement advertisement)
        {
            if (id != advertisement.AdvId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Generate slug from the updated name
                    string newSlug = advertisement.Name.ToLower().Replace(" ", "-");

                    // Check if the slug is already used by another advertisement
                    var existingAdvertisement = await _context.Advertisements
                        .FirstOrDefaultAsync(a => a.Slug == newSlug && a.AdvId != advertisement.AdvId);

                    if (existingAdvertisement != null)
                    {
                        TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Advertisement already exists!"));
                        return View(advertisement);
                    }

                    advertisement.Slug = newSlug;

                    //Change Photo
                    if (advertisement.ImageUpload != null && advertisement.ImageUpload.Length > 0)
                    {
                        string imageName = await _settingImg.ChangeImageAsync(advertisement.Image, advertisement.ImageUpload, "img/AdvImg");
                        advertisement.Image = imageName;
                    }

                    advertisement.UpdateDate = DateTime.Now;
                    advertisement.UpdatedBy = "Admin1";

                    _context.Update(advertisement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdvertisementExists(advertisement.AdvId))
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

            var advertisement = await _context.Advertisements
                .FirstOrDefaultAsync(m => m.AdvId == id);
            if (advertisement == null)
            {
                return NotFound();
            }

            ViewBag.Status = new Dictionary<int, string>
            {
                    { 1, "Available" },
                    { 0, "Unavailable" },
            };

            return View(advertisement);
        }

        // POST: Admin/Advertisements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var advertisement = await _context.Advertisements.FindAsync(id);
            if (advertisement != null)
            {
                //Delete image
                await _settingImg.DeleteImageAsync(advertisement.Image, "img/AdvImg/");
                _context.Advertisements.Remove(advertisement);
            }

            await _context.SaveChangesAsync();
            TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Deleted successfully"));
            return RedirectToAction(nameof(Index));
        }

        private bool AdvertisementExists(int id)
        {
            return _context.Advertisements.Any(e => e.AdvId == id);
        }

        public async Task<IActionResult> StatusChange(int id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var advertisement = await _context.Advertisements
                .FirstOrDefaultAsync(m => m.AdvId == id);

            if (advertisement == null)
            {
                return RedirectToAction(nameof(Index));
            }

            advertisement.Status = (advertisement.Status == 1) ? 0 : 1;
            advertisement.UpdateDate = DateTime.Now;
            advertisement.UpdatedBy = "Admin1";

            _context.Update(advertisement);
            await _context.SaveChangesAsync();

            TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Edited successfully"));
            return RedirectToAction(nameof(Index));
        }
    }
}
