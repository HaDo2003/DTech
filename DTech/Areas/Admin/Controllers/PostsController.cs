using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DTech.Models.EF;
using DTech.Library;
using Newtonsoft.Json;
using DTech.DAO;
using System.Threading.Tasks;
using DTech.Library.Service;

namespace DTech.Areas.Admin.Controllers
{
    [Area("Admin")]
    [SetViewBagAttributes]
    public class PostsController(
        PostDAO postDAO,
        PostCategoryDAO postCategoryDAO,
        CloudinaryService cloudinaryService
    ) : Controller
    {
        readonly string folderName = "Pre-thesis/Post";
        private List<PostCategory> postCategories = [];

        // Helper function to load categories if not already loaded
        private async Task LoadCategoriesAsync()
        {
            if (postCategories.Count == 0)
            {
                postCategories = await postCategoryDAO.GetListAsync();
            }
        }

        // GET: Admin/Posts
        public async Task<IActionResult> Index()
        {
            return View(await postDAO.GetListAsync());
        }

        // GET: Admin/Posts/Create
        public async Task<IActionResult> Create()
        {
            await LoadCategoriesAsync();
            ViewData["CateId"] = new SelectList(postCategories, "CateId", "Name");
            return View();
        }

        // POST: Admin/Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,CateId,Name,Slug,Image,Description,PostDate,PostBy,Status,ImageUpload")] Post post)
        {
            if (ModelState.IsValid)
            {
                //Check if adv already exist
                post.Slug = post.Name?.ToLower().Replace(" ", "-");

                var slug = await postDAO.CheckSlugAsync(post.Slug);

                if (slug != null)
                {
                    TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Post already exists!"));
                    return View(post);
                }

                //Logo Upload
                string imageName;
                if (post.ImageUpload != null && post.ImageUpload.Length > 0)
                {
                    imageName = await cloudinaryService.UploadImageAsync(post.ImageUpload, folderName);
                }
                else
                {
                    imageName = "noimg.png";
                }

                //Save to database
                post.Image = imageName;
                post.PostDate = DateTime.Now;
                post.PostBy = "Admin1";

                await postDAO.AddAsync(post);

                //Success message
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Created successfully"));
                return RedirectToAction(nameof(Index));
            }
            await LoadCategoriesAsync();
            ViewData["CateId"] = new SelectList(postCategories, "CateId", "Name", post.CateId);
            TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Create fail, please check again!"));
            return View(post);
        }

        // GET: Admin/Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await postDAO.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            await LoadCategoriesAsync();
            ViewData["CateId"] = new SelectList(postCategories, "CateId", "Name", post.CateId);
            return View(post);
        }

        // POST: Admin/Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,CateId,Name,Slug,Image,Description,PostDate,PostBy,Status,ImageUpload")] Post post)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Generate slug from the updated name
                    string newSlug = post.Name?.ToLower().Replace(" ", "-") ?? string.Empty;

                    // Check if the slug is already used by another brand
                    var existingBrand = await postDAO.CheckSlugAsync(newSlug, post.PostId);

                    if (existingBrand != null)
                    {
                        TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Post already exists!"));
                        return View(post);
                    }

                    post.Slug = newSlug;

                    //Change Photo
                    if (post.ImageUpload != null && post.ImageUpload.Length > 0)
                    {
                        string imageName = await cloudinaryService.ChangeImageAsync(
                            post.Image ?? string.Empty, 
                            post.ImageUpload, 
                            folderName);
                        post.Image = imageName;
                    }

                    await postDAO.UpdateAsync(post);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await postDAO.CheckIdAsync(post.PostId))
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
            ViewData["CateId"] = new SelectList(postCategories, "CateId", "Name", post.CateId);
            TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Edit fail, please check again!"));
            return View(post);
        }

        // GET: Admin/Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await postDAO.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Admin/Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await postDAO.GetByIdAsync(id);
            var deleteImageResult = false;
            var deletePostResult = false;

            if (post != null)
            {
                //Delete image
                if (!string.IsNullOrEmpty(post.Image))
                {
                    //Delete image
                    deleteImageResult = await cloudinaryService.DeleteImageAsync(post.Image);
                }
                else
                {
                    deleteImageResult = true; // No image to delete, consider it successful
                }

                deletePostResult = await postDAO.DeleteAsync(post);
            }
            if (deleteImageResult && deletePostResult)
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

            var post = await postDAO.GetByIdAsync(id);

            if (post == null)
            {
                return RedirectToAction(nameof(Index));
            }

            post.Status = (post.Status == 1) ? 0 : 1;

            await postDAO.UpdateAsync(post);

            TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Edited successfully"));
            return RedirectToAction(nameof(Index));
        }
    }
}
