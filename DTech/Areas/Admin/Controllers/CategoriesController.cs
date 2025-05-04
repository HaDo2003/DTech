using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DTech.Models.EF;
using DTech.Library;
using Newtonsoft.Json;
using DTech.DAO;
using Microsoft.AspNetCore.Authorization;

namespace DTech.Areas.Admin.Controllers
{
    [Area("Admin")]
    [SetViewBagAttributes]
    [Authorize(Roles = "Admin,Seller")]
    public class CategoriesController(
        CategoryDAO categoryDAO,
        LinkDAO linkDAO
    ) : Controller
    {
        private List<Category> categories = [];

        // Helper function to load categories if not already loaded
        private async Task LoadCategoriesAsync()
        {
            if (categories.Count == 0)
            {
                categories = await categoryDAO.GetListAsync();
            }
        }

        // GET: Admin/Categories
        public async Task<IActionResult> Index()
        {
            return View(await categoryDAO.GetListAsync());
        }

        // GET: Admin/Categories/Create
        public async Task<IActionResult> Create()
        {
            await LoadCategoriesAsync();
            ViewData["ParentId"] = new SelectList(categories, "CategoryId", "Name");

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
                category.Slug = category.Name?.ToLower().Replace(" ", "-");

                var slug = await categoryDAO.CheckSlugAsync(category.Slug);

                if (slug != null)
                {
                    TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Category already exists!"));
                    return View(category);
                }

                //Save to database
                category.Status ??= 1;
                category.CreateDate = DateTime.Now;
                category.CreatedBy = "Admin1";

                await categoryDAO.AddAsync(category);

                Link link = new()
                {
                    Slug = category.Slug,
                    TableId = category.CategoryId,
                    TypeLink = "Category"
                };

                await linkDAO.AddAsync(link);

                //Success message
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Created successfully"));

                return RedirectToAction(nameof(Index));
            }
            await LoadCategoriesAsync();
            ViewData["ParentId"] = new SelectList(categories, "CategoryId", "Name", category.ParentId);
            
            TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Create fail, please check again!"));
            return View(category);
        }

        // GET: Admin/Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await categoryDAO.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            await LoadCategoriesAsync();
            ViewData["ParentId"] = new SelectList(categories, "CategoryId", "Name", category.ParentId);

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
                    // Generate slug from the updated name
                    string newSlug = category.Name?.ToLower().Replace(" ", "-") ?? string.Empty;

                    // Check if the slug is already used by another category
                    var existingCategory = await categoryDAO.CheckSlugAsync(newSlug, category.CategoryId);

                    if (existingCategory != null)
                    {
                        TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Category already exists!"));
                        return View(category);
                    }

                    category.Slug = newSlug;

                    category.UpdateDate = DateTime.Now;
                    category.UpdatedBy = "Admin1";

                    await categoryDAO.UpdateAsync(category);

                    var link = await linkDAO.GetByIdAsync(category.CategoryId, "Category");

                    if (link != null)
                    {
                        link.Slug = category.Slug;
                        await linkDAO.UpdateAsync(link);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await categoryDAO.CheckIdAsync(category.CategoryId))
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
            await LoadCategoriesAsync();
            ViewData["ParentId"] = new SelectList(categories, "CategoryId", "Name", category.ParentId);

            TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Edit fail, please check again!"));
            return View(category);
        }

        // GET: Admin/Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await categoryDAO.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await categoryDAO.GetByIdAsync(id);
            if (category != null)
            {
                var link = await linkDAO.GetByIdAsync(category.CategoryId, "Category");
                await categoryDAO.DeleteAsync(category);

                if (link != null)
                {
                    await linkDAO.DeleteAsync(link);
                }
            }

            TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Deleted successfully"));
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> StatusChange(int id)
        {
            var category = await categoryDAO.GetByIdAsync(id);

            if (category == null)
            {
                return RedirectToAction(nameof(Index));
            }

            category.Status = (category.Status == 1) ? 0 : 1;
            category.UpdateDate = DateTime.Now;
            category.UpdatedBy = "Admin1";

            await categoryDAO.UpdateAsync(category);

            TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Edited successfully"));
            return RedirectToAction(nameof(Index));
        }
    }
}
