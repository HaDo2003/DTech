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
    public class AdminsController : Controller
    {
        private readonly EcommerceWebContext _context;
        private readonly ImageSetter _settingImg;

        public AdminsController(EcommerceWebContext context, ImageSetter settingImg)
        {
            _context = context;
            _settingImg = settingImg;
        }

        // GET: Admin/Admins
        public async Task<IActionResult> Index()
        {
            return View(await _context.Admins.ToListAsync());
        }

        // GET: Admin/Admins/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
                .FirstOrDefaultAsync(m => m.AdminId == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        [HttpGet]
        // GET: Admin/Admins/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Admins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AdminId,Account,Password,FirstName,LastName,Gender,Photo,CreatedBy,CreateDate,UpdatedBy,UpdateDate,PhotoUpload")] DTech.Models.EF.Admin admin)
        {
            if (ModelState.IsValid)
            {
                //Check account existed
                var existingAccount = await _context.Admins
                    .FirstOrDefaultAsync(a => a.Account == admin.Account);

                if (existingAccount != null)
                {
                    TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Account already exists!"));
                    return View(admin);
                }

                //Photo Upload
                string imageName = await _settingImg.UploadImageAsync(admin.PhotoUpload, "img/AdminImg");

                //Save to database
                admin.Photo = imageName;
                admin.CreateDate = DateTime.Now;
                admin.CreatedBy = "Admin1";

                _context.Add(admin);
                await _context.SaveChangesAsync();

                //Success message
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Created successfully"));

                return RedirectToAction(nameof(Index));
            }
            return View(admin);
        }

        // GET: Admin/Admins/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }
            return View(admin);
        }

        // POST: Admin/Admins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AdminId,Account,Password,FirstName,LastName,Gender,Photo,CreatedBy,CreateDate,UpdatedBy,UpdateDate,PhotoUpload")] DTech.Models.EF.Admin admin)
        {
            if (id != admin.AdminId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //Change Photo
                    if (admin.PhotoUpload != null && admin.PhotoUpload.Length > 0)
                    {
                        string imageName = await _settingImg.ChangeImageAsync(admin.Photo, admin.PhotoUpload, "img/AdminImg");
                        admin.Photo = imageName;
                    }

                    admin.UpdateDate = DateTime.Now;
                    admin.UpdatedBy = "Admin1";

                    _context.Update(admin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminExists(admin.AdminId))
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
            return View(admin);
        }

        // GET: Admin/Admins/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
                .FirstOrDefaultAsync(m => m.AdminId == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // POST: Admin/Admins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin != null)
            {
                //Delete image
                await _settingImg.DeleteImageAsync(admin.Photo, "img/AdminImg/");
                _context.Admins.Remove(admin);
            }

            await _context.SaveChangesAsync();
            TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Deleted successfully"));
            return RedirectToAction(nameof(Index));
        }

        private bool AdminExists(int id)
        {
            return _context.Admins.Any(e => e.AdminId == id);
        }
    }
}
