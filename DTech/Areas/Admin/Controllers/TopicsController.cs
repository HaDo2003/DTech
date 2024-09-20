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
    public class TopicsController : Controller
    {
        private readonly EcommerceWebContext _context;

        public TopicsController(EcommerceWebContext context)
        {
            _context = context;
        }

        // GET: Admin/Topics
        public async Task<IActionResult> Index()
        {
            return View(await _context.Topics.ToListAsync());
        }

        // GET: Admin/Topics/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topic = await _context.Topics
                .Include(c => c.Parent)
                .FirstOrDefaultAsync(m => m.TopicId == id);
            if (topic == null)
            {
                return NotFound();
            }

            return View(topic);
        }

        // GET: Admin/Topics/Create
        public IActionResult Create()
        {
            ViewData["ParentId"] = new SelectList(_context.Topics, "TopicId", "Name");

            return View();
        }

        // POST: Admin/Topics/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TopicId,Name,Slug,ParentId,Orders,CreatedBy,CreateDate,UpdatedBy,UpdateDate,Status")] Topic topic)
        {
            if (ModelState.IsValid)
            {
                //Check if Category already exist
                topic.Slug = topic.Name.ToLower().Replace(" ", "-");

                var slug = await _context.Topics
                    .FirstOrDefaultAsync(a => a.Slug == topic.Slug);

                if (slug != null)
                {
                    TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Post category already exists!"));
                    return View(topic);
                }

                //Save to database     
                topic.CreateDate = DateTime.Now;
                topic.CreatedBy = "Admin1";

                _context.Add(topic);
                await _context.SaveChangesAsync();

                //Success message
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Created successfully"));

                return RedirectToAction(nameof(Index));
            }
            ViewData["ParentId"] = new SelectList(_context.Topics, "TopicId", "Name", topic.ParentId);

            TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Create fail, please check again!"));
            return View(topic);
        }

        // GET: Admin/Topics/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topic = await _context.Topics.FindAsync(id);
            if (topic == null)
            {
                return NotFound();
            }

            ViewData["ParentId"] = new SelectList(_context.Topics, "TopicId", "Name");

            return View(topic);
        }

        // POST: Admin/Topics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TopicId,Name,Slug,ParentId,Orders,CreatedBy,CreateDate,UpdatedBy,UpdateDate,Status")] Topic topic)
        {
            if (id != topic.TopicId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Generate slug from the updated name
                    string newSlug = topic.Name.ToLower().Replace(" ", "-");

                    // Check if the slug is already used by another category
                    var existingBrand = await _context.Topics
                        .FirstOrDefaultAsync(a => a.Slug == newSlug && a.TopicId != topic.TopicId);

                    if (existingBrand != null)
                    {
                        TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Topic already exists!"));
                        return View(topic);
                    }

                    topic.Slug = newSlug;

                    topic.UpdateDate = DateTime.Now;
                    topic.UpdatedBy = "Admin1";

                    _context.Update(topic);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TopicExists(topic.TopicId))
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

            ViewData["ParentId"] = new SelectList(_context.Topics, "TopicId", "Name", topic.ParentId);

            TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Edit fail, please check again!"));
            return View(topic);
        }

        // GET: Admin/Topics/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topic = await _context.Topics
                .Include(c => c.Parent)
                .FirstOrDefaultAsync(m => m.TopicId == id);
            if (topic == null)
            {
                return NotFound();
            }

            return View(topic);
        }

        // POST: Admin/Topics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var topic = await _context.Topics.FindAsync(id);
            if (topic != null)
            {
                _context.Topics.Remove(topic);
            }

            await _context.SaveChangesAsync();
            TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Deleted successfully"));
            return RedirectToAction(nameof(Index));
        }

        private bool TopicExists(int id)
        {
            return _context.Topics.Any(e => e.TopicId == id);
        }
    }
}
