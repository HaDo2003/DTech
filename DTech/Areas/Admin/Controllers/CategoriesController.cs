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
    public class CategoriesController : Controller
    {
        private readonly EcommerceWebContext _context;

        public CategoriesController(EcommerceWebContext context)
        {
            _context = context;
        }

        // GET: Admin/Categories
        public async Task<IActionResult> Index()
        {
            var ecommerceWebContext = _context.Categories.Include(c => c.Parent);
            return View(await ecommerceWebContext.ToListAsync());
        }

        // GET: Admin/Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.Parent)
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }
            ViewBag.Status = new Dictionary<int, string>
            {
                { 1, "Available" },
                { 0, "Unavailable" },
            };
            return View(category);
        }

        // GET: Admin/Categories/Create
        public IActionResult Create()
        {
            ViewData["ParentId"] = new SelectList(_context.Categories, "CategoryId", "Name");

            ViewBag.Status = new List<SelectListItem>
            {
                new() { Value = "1", Text = "Available" },
                new() { Value = "0", Text = "Unavailable" },
            };

            return View();
        }

        // POST: Admin/Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,Name,ParentId,Slug,CreatedBy,CreateDate,UpdatedBy,UpdateDate,Status")] Category category)
        {
            if (ModelState.IsValid)
            {
                //Check if Category already exist
                category.Slug = category.Name.ToLower().Replace(" ", "-");

                var slug = await _context.Categories
                    .FirstOrDefaultAsync(a => a.Slug == category.Slug);

                if (slug != null)
                {
                    TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Category already exists!"));
                    return View(category);
                }

                //Save to database
                if(category.Status == null)
                {
                    category.Status = 1;
                }
                category.CreateDate = DateTime.Now;
                category.CreatedBy = "Admin1";

                _context.Add(category);
                await _context.SaveChangesAsync();

                //Success message
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Created successfully"));

                return RedirectToAction(nameof(Index));
            }
            ViewData["ParentId"] = new SelectList(_context.Categories, "CategoryId", "Name", category.ParentId);
            
            ViewBag.Status = new List<SelectListItem>
            {
                new() { Value = "1", Text = "Available" },
                new() { Value = "0", Text = "Unavailable" },
            };

            return View(category);
        }

        // GET: Admin/Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            ViewData["ParentId"] = new SelectList(_context.Categories, "CategoryId", "Name", category.ParentId);

            ViewBag.Status = new List<SelectListItem>
            {
                new() { Value = "1", Text = "Available" },
                new() { Value = "0", Text = "Unavailable" },
            };

            return View(category);
        }

        // POST: Admin/Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,Name,ParentId,Slug,CreatedBy,CreateDate,UpdatedBy,UpdateDate,Status")] Category category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //Check if category already exist
                    category.Slug = category.Name.ToLower().Replace(" ", "-");

                    var slug = await _context.Categories
                        .FirstOrDefaultAsync(a => a.Slug == category.Slug);

                    if (slug != null)
                    {
                        TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Category already exists!"));
                        return View(category);
                    }

                    category.UpdateDate = DateTime.Now;
                    category.UpdatedBy = "Admin1";

                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
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
            ViewData["ParentId"] = new SelectList(_context.Categories, "CategoryId", "Name", category.ParentId);

            ViewBag.Status = new List<SelectListItem>
            {
                new() { Value = "1", Text = "Available" },
                new() { Value = "0", Text = "Unavailable" },
            };

            return View(category);
        }

        // GET: Admin/Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.Parent)
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            ViewBag.Status = new Dictionary<int, string>
            {
                    { 1, "Available" },
                    { 0, "Unavailable" },
            };

            return View(category);
        }

        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }

            await _context.SaveChangesAsync();
            TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Deleted successfully"));
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }

        public async Task<IActionResult> StatusChange(int id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);

            if (category == null)
            {
                return RedirectToAction(nameof(Index));
            }

            category.Status = (category.Status == 1) ? 0 : 1;
            category.UpdateDate = DateTime.Now;
            category.UpdatedBy = "Admin1";

            _context.Update(category);
            await _context.SaveChangesAsync();

            TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Edited successfully"));
            return RedirectToAction(nameof(Index));
        }
    }
}
