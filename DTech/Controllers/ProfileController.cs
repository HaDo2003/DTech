using Microsoft.AspNetCore.Mvc;
using DTech.Models;
using DTech.DAO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using DTech.Models.EF;
using CloudinaryDotNet.Actions;
using DTech.Library.Service;
using DTech.Library;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DTech.Controllers
{
    [Authorize]
    [Route("profile")]
    public class ProfileController(
        CloudinaryService cloudinaryService,
        CustomerDAO customerDAO
    ) : Controller
    {
        readonly string folderName = "Pre-thesis/Customer";


        [Route("")]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            ViewData["ActionName"] = "Profile";
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Authentication");
            }

            var user = await customerDAO.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [Route("")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            [Bind("Id,FullName,UserName,Gender,DateOfBirth,PhoneNumber,Email,Image,UpdatedBy,UpdateDate,ImageUpload")]
            ApplicationUser user)
        {
            ViewData["ActionName"] = "Profile";
            if (ModelState.IsValid)
            {
                try
                {
                    // Check email existed
                    var existingEmail = await customerDAO.CheckEmailAsync(user.Email, user.Id);
                    if (existingEmail)
                    {
                        ModelState.AddModelError("Email", "Email already exists.");
                        return View("Profile", user);
                    }

                    // Check phone existed
                    var existingPhone = await customerDAO.CheckPhoneAsync(user.PhoneNumber, user.Id);
                    if (existingPhone)
                    {
                        ModelState.AddModelError("PhoneNumber", "Phone Number already exists.");
                        return View("Profile", user);
                    }

                    // Handle image change
                    if (user.ImageUpload != null && user.ImageUpload.Length > 0)
                    {
                        string imageName = await cloudinaryService.ChangeImageAsync(
                            oldfile: user.Image ?? string.Empty,
                            newfile: user.ImageUpload,
                            filepath: folderName
                        );
                        user.Image = imageName;
                    }

                    user.UpdateDate = DateTime.Now;
                    user.UpdatedBy = user.UserName;

                    bool result = await customerDAO.UpdateAsync(user);
                    if (result)
                    {
                        return View("Profile", user);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await customerDAO.CheckIdAsync(user.Id)) return NotFound();
                    else throw;
                }
            }

            TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Edit failed, please check again!"));
            return View("Profile", user);
        }

        [HttpGet]
        [Route("orders")]
        public async Task<IActionResult> Orders()
        {
            ViewData["ActionName"] = "Orders";
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Authentication");

            var orders = await customerDAO.GetOrdersByCustomerIdAsync(userId);
            return PartialView("Orders", orders);
        }
    }
}
