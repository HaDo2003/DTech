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
    public class BrandsController : Controller
    {
        private readonly EcommerceWebContext _context;
        private readonly ImageSetter _settingImg;

        public BrandsController(EcommerceWebContext context, ImageSetter settingImg)
        {
            _context = context;
            _settingImg = settingImg;
        }

        // GET: Admin/Brands
        public async Task<IActionResult> Index()
        {
            return View(await _context.Brands.ToListAsync());
        }

        // GET: Admin/Brands/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands
                .FirstOrDefaultAsync(m => m.BrandId == id);
            if (brand == null)
            {
                return NotFound();
            }

            ViewBag.Status = new Dictionary<int, string>
            {
                { 1, "Available" },
                { 0, "Unavailable" },
            };

            return View(brand);
        }

        // GET: Admin/Brands/Create
        public IActionResult Create()
        {
            ViewBag.Status = new List<SelectListItem>
            {
                new() { Value = "1", Text = "Available" },
                new() { Value = "0", Text = "Unavailable" },
            };
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
                //Check if adv already exist
                brand.Slug = brand.Name.ToLower().Replace(" ", "-");

                var slug = await _context.Brands
                    .FirstOrDefaultAsync(a => a.Slug == brand.Slug);

                if (slug != null)
                {
                    TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Brand already exists!"));
                    return View(brand);
                }

                //Logo Upload
                string imageName = await _settingImg.UploadImageAsync(brand.LogoUpload, "img/BrandLogo");

                //Save to database
                brand.Logo = imageName;
                brand.CreateDate = DateTime.Now;
                brand.CreatedBy = "Admin1";

                _context.Add(brand);
                await _context.SaveChangesAsync();

                //Success message
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Created successfully"));
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Status = new List<SelectListItem>
            {
                new() { Value = "1", Text = "Available" },
                new() { Value = "0", Text = "Unavailable" },
            };
            return View(brand);
        }

        // GET: Admin/Brands/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }
            ViewBag.Status = new List<SelectListItem>
            {
                new() { Value = "1", Text = "Available" },
                new() { Value = "0", Text = "Unavailable" },
            };
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
                    string newSlug = brand.Name.ToLower().Replace(" ", "-");

                    // Check if the slug is already used by another brand
                    var existingBrand = await _context.Brands
                        .FirstOrDefaultAsync(a => a.Slug == newSlug && a.BrandId != brand.BrandId);

                    if (existingBrand != null)
                    {
                        TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Brand already exists!"));
                        return View(brand);
                    }

                    brand.Slug = newSlug;

                    //Change Photo
                    if (brand.LogoUpload != null && brand.LogoUpload.Length > 0)
                    {
                        string imageName = await _settingImg.ChangeImageAsync(brand.Logo, brand.LogoUpload, "img/BrandLogo");
                        brand.Logo = imageName;
                    }
                    
                    brand.UpdateDate = DateTime.Now;
                    brand.UpdatedBy = "Admin1";

                    _context.Update(brand);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BrandExists(brand.BrandId))
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
            ViewBag.Status = new List<SelectListItem>
            {
                new() { Value = "1", Text = "Available" },
                new() { Value = "0", Text = "Unavailable" },
            };
            return View(brand);
        }

        // GET: Admin/Brands/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands
                .FirstOrDefaultAsync(m => m.BrandId == id);
            if (brand == null)
            {
                return NotFound();
            }

            ViewBag.Status = new Dictionary<int, string>
            {
                { 1, "Available" },
                { 0, "Unavailable" },
            };

            return View(brand);
        }

        // POST: Admin/Brands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand != null)
            {
                //Delete image
                await _settingImg.DeleteImageAsync(brand.Logo, "img/BrandLogo/");
                _context.Brands.Remove(brand);
            }

            await _context.SaveChangesAsync();
            TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Deleted successfully"));
            return RedirectToAction(nameof(Index));
        }

        private bool BrandExists(int id)
        {
            return _context.Brands.Any(e => e.BrandId == id);
        }

        public async Task<IActionResult> StatusChange(int id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var brand = await _context.Brands
                .FirstOrDefaultAsync(m => m.BrandId == id);

            if (brand == null)
            {
                return RedirectToAction(nameof(Index));
            }

            brand.Status = (brand.Status == 1) ? 0 : 1;
            brand.UpdateDate = DateTime.Now;
            brand.UpdatedBy = "Admin1";

            _context.Update(brand);
            await _context.SaveChangesAsync();

            TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Edited successfully"));
            return RedirectToAction(nameof(Index));
        }
    }
}