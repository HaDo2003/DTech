
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DTech.Models.EF;
using DTech.Library;
using Newtonsoft.Json;
using DTech.DAO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using DTech.Library.Service;
using Microsoft.AspNetCore.Authorization;

namespace DTech.Areas.Admin.Controllers
{
    [Area("Admin")]
    [SetViewBagAttributes]
    [Authorize(Roles = "Admin")]
    public class AdminsController(
        CloudinaryService cloudinaryService,
        AdminDAO adminDAO,
        RoleDAO roleDAO
    ) : Controller
    {
        readonly string folderName = "Pre-thesis/Admin";

        // GET: Admin/Admins
        public async Task<IActionResult> Index()
        {
            return View(await adminDAO.GetListAsync());
        }

        [HttpGet]
        // GET: Admin/Admins/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Role = new SelectList(await roleDAO.GetListAsync(), "Id", "Name");
            return View();
        }

        // Updated code in the Create method to handle potential null reference for 'admin.PhotoUpload'
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,RoleId,UserName,Gender,DateOfBirth,PhoneNumber,Email,PasswordHash,Image,CreatedBy,CreateDate,UpdatedBy,UpdateDate,ImageUpload,Address")]
            ApplicationUser admin)
        {
            if (ModelState.IsValid)
            {
                // Check account existed
                var existingAccount = await adminDAO.CheckAccountAsync(admin.Email);

                if (existingAccount)
                {
                    TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Account already exists!"));
                    return View(admin);
                }

                // Photo Upload
                string imageName;
                if (admin.ImageUpload != null && admin.ImageUpload.Length > 0)
                {
                    imageName = await cloudinaryService.UploadImageAsync(admin.ImageUpload, folderName);
                }
                else
                {
                    imageName = "noimg.png";
                }

                // Save to database
                admin.Image = imageName;
                admin.CreateDate = DateTime.Now;
                admin.CreatedBy = "Admin1";

                bool result = await adminDAO.AddAsync(admin);

                // Success message
                if (result)
                {
                    TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Created successfully"));
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewBag.Role = new SelectList(await roleDAO.GetListAsync(), "Id", "Name");
            TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Create failed, please check again!"));
            return View(admin);
        }

        // GET: Admin/Admins/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await adminDAO.GetByIdAsync(id);
            if (admin == null)
            {
                return NotFound();
            }
            var roles = await roleDAO.GetListAsync();
            ViewBag.Role = new SelectList(roles, "Id", "Name", admin.RoleId);
            return View(admin);
        }

        // Updated code in the Edit method to handle potential null reference for 'admin.Photo'
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id,
        [Bind("Id,RoleId,UserName,Gender,DateOfBirth,PhoneNumber,Email,PasswordHash,Image,CreatedBy,CreateDate,UpdatedBy,UpdateDate,ImageUpload,Address")]
        ApplicationUser admin,
        string RoleId)
        {
            if (id != admin.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle image change
                    if (admin.ImageUpload != null && admin.ImageUpload.Length > 0)
                    {
                        string imageName = await cloudinaryService.ChangeImageAsync(
                            oldfile: admin.Image ?? string.Empty,
                            newfile: admin.ImageUpload,
                            filepath: folderName
                        );
                        admin.Image = imageName;
                    }

                    admin.UpdateDate = DateTime.Now;
                    admin.UpdatedBy = "Admin1";

                    bool result = await adminDAO.UpdateAsync(admin, RoleId);
                    if (result)
                    {
                        TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Edited successfully"));
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await adminDAO.CheckIdAsync(admin.Id)) return NotFound();
                    else throw;
                }
            }

            ViewBag.Role = new SelectList(await roleDAO.GetListAsync(), "Id", "Name");
            TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Edit failed, please check again!"));
            return View(admin);
        }

        // GET: Admin/Admins/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await adminDAO.GetByIdAsync(id);
              
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // Updated code in the DeleteConfirmed method to handle potential null reference for 'admin.Photo'
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var admin = await adminDAO.GetByIdAsync(id);
            var deleteImageResult = false;
            var deleteAdminResult = false;

            if (admin != null)
            {
                //Delete image
                if (!string.IsNullOrEmpty(admin.Image))
                {
                    deleteImageResult = await cloudinaryService.DeleteImageAsync(imageUrl: admin.Image);
                }
                else
                {
                    deleteImageResult = true;
                }

                deleteAdminResult = await adminDAO.DeleteAsync(admin);
            }

            if (deleteImageResult && deleteAdminResult)
            {
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Deleted successfully"));
            }
            else
            {
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Delete failed, please check again!"));
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole([Bind("Id,Name")] IdentityRole role)
        {
            if (ModelState.IsValid)
            {
                // Check account existed
                var existingRole = await roleDAO.CheckRoleAsync(role.Name);

                if (existingRole)
                {
                    TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Role already exists!"));
                    return View(role);
                }

               // Save to database
                bool result = await roleDAO.AddAsync(role);

                // Success message
                if (result)
                {
                    TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Role is created successfully"));
                    return RedirectToAction(nameof(Index));
                }
            }
            TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Role is create failed, please check again!"));
            return View(role);
        }
    }
}
