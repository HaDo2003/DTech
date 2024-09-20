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
                //Check if Category already exist
                postCategory.Slug = postCategory.Name.ToLower().Replace(" ", "-");

                var slug = await _context.PostCategories
                    .FirstOrDefaultAsync(a => a.Slug == postCategory.Slug);

                if (slug != null)
                {
                    TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Post category already exists!"));
                    return View(postCategory);
                }

                //Save to database     
                postCategory.CreateDate = DateTime.Now;
                postCategory.CreatedBy = "Admin1";

                _context.Add(postCategory);
                await _context.SaveChangesAsync();

                //Success message
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Created successfully"));

                return RedirectToAction(nameof(Index));
            }

            TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Create fail, please check again!"));
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
                    // Generate slug from the updated name
                    string newSlug = postCategory.Name.ToLower().Replace(" ", "-");

                    // Check if the slug is already used by another category
                    var existingBrand = await _context.PostCategories
                        .FirstOrDefaultAsync(a => a.Slug == newSlug && a.CateId != postCategory.CateId);

                    if (existingBrand != null)
                    {
                        TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Post category already exists!"));
                        return View(postCategory);
                    }

                    postCategory.Slug = newSlug;

                    postCategory.UpdateDate = DateTime.Now;
                    postCategory.UpdatedBy = "Admin1";

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
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Edited successfully"));
                return RedirectToAction(nameof(Index));
            }

            TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Edit fail, please check again!"));
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
            TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Deleted successfully"));
            return RedirectToAction(nameof(Index));
        }

        private bool PostCategoryExists(int id)
        {
            return _context.PostCategories.Any(e => e.CateId == id);
        }

        public async Task<IActionResult> StatusChange(int id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var postCategory = await _context.PostCategories
                .FirstOrDefaultAsync(m => m.CateId == id);

            if (postCategory == null)
            {
                return RedirectToAction(nameof(Index));
            }

            postCategory.Status = (postCategory.Status == 1) ? 0 : 1;
            postCategory.UpdateDate = DateTime.Now;
            postCategory.UpdatedBy = "Admin1";

            _context.Update(postCategory);
            await _context.SaveChangesAsync();

            TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Edited successfully"));
            return RedirectToAction(nameof(Index));
        }
    }
}
