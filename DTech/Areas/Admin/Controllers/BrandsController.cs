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
        private readonly IWebHostEnvironment _environment;

        public BrandsController(EcommerceWebContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
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

            return View(brand);
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
        public async Task<IActionResult> Create([Bind("BrandId,Name,Slug,Logo,CreatedBy,CreateDate,UpdatedBy,UpdateDate,LogoUpload")] Brand brand)
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

                //ImageUpload
                string imageName;

                if (brand.LogoUpload != null)
                {
                    string uploadDir = Path.Combine(_environment.WebRootPath, "img/BrandLogo");
                    imageName = Path.GetFileName(brand.LogoUpload.FileName);

                    string filePath = Path.Combine(uploadDir, imageName);

                    using (var fs = new FileStream(filePath, FileMode.Create))
                    {
                        await brand.LogoUpload.CopyToAsync(fs);
                    }

                }
                else
                {
                    imageName = "noimgge.png";
                }

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
            return View(brand);
        }

        // POST: Admin/Brands/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BrandId,Name,Slug,Logo,CreatedBy,CreateDate,UpdatedBy,UpdateDate,LogoUpload")] Brand brand)
        {
            if (id != brand.BrandId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //Change name
                    brand.Slug = brand.Name.ToLower().Replace(" ", "-");
                    var slug = await _context.Brands
                    .FirstOrDefaultAsync(a => a.Slug == brand.Slug);

                    if (slug != null)
                    {
                        TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Brand already exists!"));
                        return View(brand);
                    }

                    //Change Photo
                    if (brand.LogoUpload != null && brand.LogoUpload.Length > 0)
                    {
                        string uploadDir = Path.Combine(_environment.WebRootPath, "img/BrandLogo");

                        //Delete old imgae
                        if (!string.Equals(brand.Logo, "noimage.png"))
                        {
                            string oldImagePath = Path.Combine(uploadDir, brand.Logo);

                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }
                        string imageName = Path.GetFileName(brand.LogoUpload.FileName);

                        string filePath = Path.Combine(uploadDir, imageName);

                        using (var fs = new FileStream(filePath, FileMode.Create))
                        {
                            await brand.LogoUpload.CopyToAsync(fs);
                        }

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
                //Delete old image
                if (!string.Equals(brand.Logo, "noimage.png"))
                {
                    string PhotoPath = Path.Combine(_environment.WebRootPath, "img/BrandLogo/" + brand.Logo);

                    if (System.IO.File.Exists(PhotoPath))
                    {
                        System.IO.File.Delete(PhotoPath);
                    }
                }
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
    }
}
