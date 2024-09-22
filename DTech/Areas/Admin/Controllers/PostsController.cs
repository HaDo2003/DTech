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
using System.Drawing.Drawing2D;

namespace DTech.Areas.Admin.Controllers
{
    [Area("Admin")]
    [SetViewBagAttributes]
    public class PostsController : Controller
    {
        private readonly EcommerceWebContext _context;
        private readonly ImageSetter _settingImg;

        public PostsController(EcommerceWebContext context, ImageSetter settingImg)
        {
            _context = context;
            _settingImg = settingImg;
        }

        // GET: Admin/Posts
        public async Task<IActionResult> Index()
        {
            var ecommerceWebContext = _context.Posts.Include(p => p.Cate);
            return View(await ecommerceWebContext.ToListAsync());
        }

        // GET: Admin/Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Cate)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Admin/Posts/Create
        public IActionResult Create()
        {
            ViewData["CateId"] = new SelectList(_context.PostCategories, "CateId", "Name");
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
                post.Slug = post.Name.ToLower().Replace(" ", "-");

                var slug = await _context.Posts
                    .FirstOrDefaultAsync(a => a.Slug == post.Slug);

                if (slug != null)
                {
                    TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Post already exists!"));
                    return View(post);
                }

                //Logo Upload
                string imageName = await _settingImg.UploadImageAsync(post.ImageUpload, "img/PostImg");

                //Save to database
                post.Image = imageName;
                post.PostDate = DateTime.Now;
                post.PostBy = "Admin1";
                _context.Add(post);
                await _context.SaveChangesAsync();

                //Success message
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Created successfully"));
                return RedirectToAction(nameof(Index));
            }
            ViewData["CateId"] = new SelectList(_context.PostCategories, "CateId", "Name", post.CateId);
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

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["CateId"] = new SelectList(_context.PostCategories, "CateId", "Name", post.CateId);
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
                    string newSlug = post.Name.ToLower().Replace(" ", "-");

                    // Check if the slug is already used by another brand
                    var existingBrand = await _context.Posts
                        .FirstOrDefaultAsync(a => a.Slug == newSlug && a.PostId != post.PostId);

                    if (existingBrand != null)
                    {
                        TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Post already exists!"));
                        return View(post);
                    }

                    post.Slug = newSlug;

                    //Change Photo
                    if (post.ImageUpload != null && post.ImageUpload.Length > 0)
                    {
                        string imageName = await _settingImg.ChangeImageAsync(post.Image, post.ImageUpload, "img/PostImg");
                        post.Image = imageName;
                    }

                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostId))
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
            ViewData["CateId"] = new SelectList(_context.PostCategories, "CateId", "Name", post.CateId);
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

            var post = await _context.Posts
                .Include(p => p.Cate)
                .FirstOrDefaultAsync(m => m.PostId == id);
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
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                //Delete image
                await _settingImg.DeleteImageAsync(post.Image, "img/PostImg/");
                _context.Posts.Remove(post);
            }

            await _context.SaveChangesAsync();
            TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Deleted successfully"));
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }

        public async Task<IActionResult> StatusChange(int id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.PostId == id);

            if (post == null)
            {
                return RedirectToAction(nameof(Index));
            }

            post.Status = (post.Status == 1) ? 0 : 1;

            _context.Update(post);
            await _context.SaveChangesAsync();

            TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Edited successfully"));
            return RedirectToAction(nameof(Index));
        }
    }
}
