using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DTech.Models.EF;

namespace DTech.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PostCategoriesController : Controller
    {
        private readonly EcommerceWebContext _context;

        public PostCategoriesController(EcommerceWebContext context)
        {
            _context = context;
        }

        // GET: Admin/PostCategories
        public async Task<IActionResult> Index()
        {
            return View(await _context.PostCategories.ToListAsync());
        }

        // GET: Admin/PostCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postCategory = await _context.PostCategories
                .FirstOrDefaultAsync(m => m.CateId == id);
            if (postCategory == null)
            {
                return NotFound();
            }

            return View(postCategory);
        }

        // GET: Admin/PostCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/PostCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CateId,Name,Slug,CreatedBy,CreateDate,UpdatedBy,UpdateDate,Status")] PostCategory postCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(postCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(postCategory);
        }

        // GET: Admin/PostCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postCategory = await _context.PostCategories.FindAsync(id);
            if (postCategory == null)
            {
                return NotFound();
            }
            return View(postCategory);
        }

        // POST: Admin/PostCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CateId,Name,Slug,CreatedBy,CreateDate,UpdatedBy,UpdateDate,Status")] PostCategory postCategory)
        {
            if (id != postCategory.CateId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(postCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostCategoryExists(postCategory.CateId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(postCategory);
        }

        // GET: Admin/PostCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postCategory = await _context.PostCategories
                .FirstOrDefaultAsync(m => m.CateId == id);
            if (postCategory == null)
            {
                return NotFound();
            }

            return View(postCategory);
        }

        // POST: Admin/PostCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var postCategory = await _context.PostCategories.FindAsync(id);
            if (postCategory != null)
            {
                _context.PostCategories.Remove(postCategory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostCategoryExists(int id)
        {
            return _context.PostCategories.Any(e => e.CateId == id);
        }
    }
}
