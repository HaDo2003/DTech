﻿using System;
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
    public class AdvertisementsController : Controller
    {
        private readonly EcommerceWebContext _context;
        private readonly IWebHostEnvironment _environment;

        public AdvertisementsController(EcommerceWebContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
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
            ViewBag.Status = new List<SelectListItem>
            {
                new() { Value = "1", Text = "Available" },
                new() { Value = "0", Text = "Unavailable" },
            };
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
                    TempData["message"] = JsonConvert.SerializeObject(new XMessage("error", "Adevertisement already exists!"));
                    return View(advertisement);
                }

                //ImageUpload
                string imageName;

                if (advertisement.ImageUpload != null)
                {
                    string uploadDir = Path.Combine(_environment.WebRootPath, "img/AdvImg");
                    imageName = Path.GetFileName(advertisement.ImageUpload.FileName);

                    string filePath = Path.Combine(uploadDir, imageName);

                    using (var fs = new FileStream(filePath, FileMode.Create))
                    {
                        await advertisement.ImageUpload.CopyToAsync(fs);
                    }

                }
                else
                {
                    imageName = "noimgge.png";
                }

                //Save to database
                advertisement.Image = imageName;
                advertisement.CreateDate = DateTime.Now;
                advertisement.CreatedBy = "Admin1";

                _context.Add(advertisement);
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
            ViewBag.Status = new List<SelectListItem>
            {
                new() { Value = "1", Text = "Available" },
                new() { Value = "0", Text = "Unavailable" },
            };
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
                    //Change Photo
                    if (advertisement.ImageUpload != null && advertisement.ImageUpload.Length > 0)
                    {
                        string uploadDir = Path.Combine(_environment.WebRootPath, "img/AdvImg");

                        //Delete old imgae
                        if (!string.Equals(advertisement.Image, "noimage.png"))
                        {
                            string oldImagePath = Path.Combine(uploadDir, advertisement.Image);

                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }
                        string imageName = Path.GetFileName(advertisement.ImageUpload.FileName);

                        string filePath = Path.Combine(uploadDir, imageName);

                        using (var fs = new FileStream(filePath, FileMode.Create))
                        {
                            await advertisement.ImageUpload.CopyToAsync(fs);
                        }

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
                //Delete old imgae
                if (!string.Equals(advertisement.Image, "noimage.png"))
                {
                    string PhotoPath = Path.Combine(_environment.WebRootPath, "img/AdvImg/" + advertisement.Image);

                    if (System.IO.File.Exists(PhotoPath))
                    {
                        System.IO.File.Delete(PhotoPath);
                    }
                }

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
