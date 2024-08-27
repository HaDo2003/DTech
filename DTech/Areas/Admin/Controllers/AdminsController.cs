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
        private readonly IWebHostEnvironment _environment;

        public AdminsController(EcommerceWebContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
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
                string imageName;

                if(admin.PhotoUpload != null)
                {
                    string uploadDir = Path.Combine(_environment.WebRootPath, "img/AdminImg");
                    imageName = Path.GetFileName(admin.PhotoUpload.FileName);

                    string filePath = Path.Combine(uploadDir, imageName);

                    using (var fs = new FileStream(filePath, FileMode.Create))
                    {
                        await admin.PhotoUpload.CopyToAsync(fs);
                    }

                }
                else
                {
                    imageName = "noimgge.png";
                }

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
                        string uploadDir = Path.Combine(_environment.WebRootPath, "img/AdminImg");

                        //Delete old imgae
                        if (!string.Equals(admin.Photo, "noimage.png"))
                        {
                            string oldImagePath = Path.Combine(uploadDir, admin.Photo);

                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }
                        string imageName = Path.GetFileName(admin.PhotoUpload.FileName);

                        string filePath = Path.Combine(uploadDir, imageName);

                        using (var fs = new FileStream(filePath, FileMode.Create))
                        {
                            await admin.PhotoUpload.CopyToAsync(fs);
                        }

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

                //Delete old imgae
                if (!string.Equals(admin.Photo, "noimage.png"))
                {
                    string PhotoPath = Path.Combine(_environment.WebRootPath, "img/AdminImg/" + admin.Photo);

                    if (System.IO.File.Exists(PhotoPath))
                    {
                        System.IO.File.Delete(PhotoPath);
                    }
                }

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
