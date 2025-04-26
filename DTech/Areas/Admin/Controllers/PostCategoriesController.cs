using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DTech.Models.EF;
using DTech.Library;
using Newtonsoft.Json;
using DTech.DAO;

namespace DTech.Areas.Admin.Controllers
{
    [Area("Admin")]
    [SetViewBagAttributes]
    public class PostCategoriesController(
        PostCategoryDAO postCategoryDAO
    ) : Controller
    {

        // GET: Admin/PostCategories
        public async Task<IActionResult> Index()
        {
            return View(await postCategoryDAO.GetListAsync());
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
        public async Task<IActionResult> Create([Bind("CategoryId,Name,Slug,CreatedBy,CreateDate,UpdatedBy,UpdateDate,Status")] PostCategory postCategory)
        {
            if (ModelState.IsValid)
            {
                //Check if Category already exist
                postCategory.Slug = postCategory.Name?.ToLower().Replace(" ", "-");

                var slug = await postCategoryDAO.CheckSlugAsync(postCategory.Slug);

                if (slug != null)
                {
                    TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Post category already exists!"));
                    return View(postCategory);
                }

                //Save to database     
                postCategory.CreateDate = DateTime.Now;
                postCategory.CreatedBy = "Admin1";

                await postCategoryDAO.AddAsync(postCategory);

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

            var postCategory = await postCategoryDAO.GetByIdAsync(id);
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
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,Name,Slug,CreatedBy,CreateDate,UpdatedBy,UpdateDate,Status")] PostCategory postCategory)
        {
            if (id != postCategory.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Generate slug from the updated name
                    string newSlug = postCategory.Name?.ToLower().Replace(" ", "-") ?? string.Empty;

                    // Check if the slug is already used by another category
                    var existingBrand = await postCategoryDAO.CheckSlugAsync(newSlug, postCategory.CategoryId);

                    if (existingBrand != null)
                    {
                        TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Post category already exists!"));
                        return View(postCategory);
                    }

                    postCategory.Slug = newSlug;

                    postCategory.UpdateDate = DateTime.Now;
                    postCategory.UpdatedBy = "Admin1";

                    await postCategoryDAO.UpdateAsync(postCategory);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await postCategoryDAO.CheckIdAsync(postCategory.CategoryId))
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

            var postCategory = await postCategoryDAO.GetByIdAsync(id);
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
            var postCategory = await postCategoryDAO.GetByIdAsync(id);
            if (postCategory != null)
            {
                await postCategoryDAO.DeleteAsync(postCategory);
            }

            TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Deleted successfully"));
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> StatusChange(int id)
        {
            var postCategory = await postCategoryDAO.GetByIdAsync(id);

            if (postCategory == null)
            {
                return RedirectToAction(nameof(Index));
            }

            postCategory.Status = (postCategory.Status == 1) ? 0 : 1;
            postCategory.UpdateDate = DateTime.Now;
            postCategory.UpdatedBy = "Admin1";

            await postCategoryDAO.UpdateAsync(postCategory);

            TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Edited successfully"));
            return RedirectToAction(nameof(Index));
        }
    }
}
