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
using DTech.Models.ViewModel;
using Microsoft.AspNetCore.Authentication;

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


        // GET: Profile
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

        // POST: Profile/Edit
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

        // GET: Profile/Orders
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

        // GET: Profile/Coupons
        [HttpGet]
        [Route("coupons")]
        public async Task<IActionResult> Coupon()
        {
            ViewData["ActionName"] = "Coupons";
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Authentication");

            var coupons = await customerDAO.GetCouponsByCustomerIdAsync(userId);
            return PartialView("Coupons", coupons);
        }

        // GET: Profile/ChangePassword
        [HttpGet]
        [Route("change-password")]
        public IActionResult ChangePassword()
        {
            ViewData["ActionName"] = "Change Password";
            var model = new ChangePasswordViewModel();
            return PartialView(model);
        }

        // POST: Profile/ChangePassword
        [HttpPost]
        [Route("change-password")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(
            [Bind("CurrentPassword,NewPassword,ConfirmPassword")]
            ChangePasswordViewModel model)
        {
            ViewData["ActionName"] = "Change Password";
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Authentication");
            if (ModelState.IsValid)
            {
                var result = await customerDAO.ChangePasswordAsync(userId, model.CurrentPassword, model.NewPassword);
                if (result)
                {
                    await HttpContext.SignOutAsync();
                    return RedirectToAction("Login", "Authentication");
                }
                else
                {
                    ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                }
            }
            ModelState.AddModelError("CurrentPassword", "Change Password Fail, please try again");
            return PartialView(model);
        }

        // GET: Profile/Address
        [HttpGet]
        [Route("address")]
        public async Task<IActionResult> Address()
        {
            ViewData["ActionName"] = "Address";
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Authentication");
            var addresses = await customerDAO.GetAddressesByCustomerIdAsync(userId);
            return PartialView("Address", addresses);
        }

        // GET: Profile/Address/Create
        [HttpGet]
        [Route("address/create")]
        public IActionResult CreateAddress()
        {
            ViewData["ActionName"] = "Create Address";
            var model = new CustomerAddress();
            return PartialView(model);
        }
    }
}
