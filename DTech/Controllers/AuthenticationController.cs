using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using DTech.Models.ViewModel;
using DTech.Models.EF;
using DTech.DAO;
using CloudinaryDotNet;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using DTech.Library.Service;
using DTech.Library;
using Newtonsoft.Json;
using NuGet.Protocol;
using Microsoft.EntityFrameworkCore;

namespace DTech.Controllers
{
    public class AuthenticationController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        CustomerDAO customerDAO,
        RoleDAO roleDAO,
        CartDAO cartDAO,
        CustomerAddressDAO customerAddressDAO,
        IEmailService emailService
    ) : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserViewModel newUser)
        {
            if (ModelState.IsValid)
            {
                var existingEmail = await customerDAO.CheckEmailAsync(newUser.Email);

                if (existingEmail)
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                    return View("Register", newUser);
                }

                var existingPhone = await customerDAO.CheckPhoneAsync(newUser.PhoneNumber);

                if (existingPhone)
                {
                    ModelState.AddModelError("PhoneNumber", "Phone Number already exists.");
                    return View("Register", newUser);
                }

                ApplicationUser user = new()
                {
                    RoleId = await roleDAO.GetCusomerRoleId("Customer") ?? string.Empty,
                    FullName = newUser.FullName,
                    Email = newUser.Email,
                    UserName = newUser.Account,
                    PhoneNumber = newUser.PhoneNumber,
                    Gender = newUser.Gender,
                    PasswordHash = newUser.Password,
                    DateOfBirth = newUser.DateOfBirth,
                    CreatedBy = newUser.FullName,
                    CreateDate = DateTime.Now,
                };

                var result = await customerDAO.AddAsync(user);

                if (result)
                {
                    //Create address
                    CustomerAddress customerAddress = new()
                    {
                        CustomerId = user.Id,
                        Address = newUser.Address,
                    };
                    await customerAddressDAO.CreateAsync(customerAddress);

                    //Create Cart for new customer
                    Cart cart = new()
                    {
                        CustomerId = user.Id
                    };
                    await cartDAO.CreateAsync(cart);

                    await signInManager.SignInAsync(user, false);

                    return RedirectToAction("Index", "Home");
                }
            }
            // If validation fails, return the view with the model to show errors
            return View("Register", newUser);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel user)
        {
            if (ModelState.IsValid)
            {
                var existingAccount = await customerDAO.CheckAccountAsync(user.Account);

                if (!existingAccount)
                {
                    ModelState.AddModelError("Account", "Account do not exist.");
                    return View("Login", user);
                }

                if (user.Password.Length < 6)
                {
                    ModelState.AddModelError("Password", "Password must be at least 6 character.");
                    return View("Login", user);
                }

                var loginResult = await signInManager.PasswordSignInAsync(user.Account, user.Password, false, false);
                if (loginResult.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("Password", "Password is not correct.");
                    return View("Login", user);
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword([Required, EmailAddress] string email)
        {
            ViewBag.Email = email;
            if (!ModelState.IsValid)
            {
                var error = ModelState["email"]?.Errors.First().ErrorMessage ?? "Invalid Email Address";
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", error));
                return View();
            }
            var user = await userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                
                string resetLink = Url.Action(
                    "ResetPassword",
                    "Authentication",
                    new { token, email = user.Email },
                    protocol: Request.Scheme
                ) ?? "URL Error";

                await emailService.SendEmailAsync(
                    email,
                    "Reset Your Password",
                    $@"
                    <html>
                        <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
                            <div style='max-width: 600px; margin: auto; background: white; padding: 20px; border-radius: 10px; box-shadow: 0 2px 5px rgba(0,0,0,0.1);'>
                                <h2 style='color: #333;'>Password Reset Request</h2>
                                <p>Hello,</p>
                                <p>We received a request to reset your password. Click the button below to proceed:</p>
                                <div style='text-align: center; margin: 30px 0;'>
                                    <a href='{resetLink}' style='background-color: #4CAF50; color: white; padding: 14px 25px; text-align: center; text-decoration: none; display: inline-block; border-radius: 5px; font-size: 16px;'>
                                        Reset Password
                                    </a>
                                </div>
                                <p>If you did not request a password reset, please ignore this email.</p>
                                <p>Thank you,<br/>DTeam</p>
                            </div>
                        </body>
                    </html>"
                ); 
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Please check your email to reset your password"));
                return View();
            }
            else
            {
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Email not found"));
                return View();
            }
        }

        [HttpGet]
        public IActionResult ResetPassword(string? token, string? email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Invalid token or email"));
                return RedirectToAction("ForgotPassword", "Authentication");
            }

            var model = new PasswordViewModel { Token = token, Email = email };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(PasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (string.IsNullOrEmpty(model.Email))
            {
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Email cannot be null or empty"));
                return View(model);
            }

            if (string.IsNullOrEmpty(model.Token))
            {
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Token cannot be null or empty"));
                return View(model);
            }

            if (string.IsNullOrEmpty(model.NewPassword))
            {
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "New password cannot be null or empty"));
                return View(model);
            }

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Invalid email address"));
                return View(model);
            }

            var result = await userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Password reset successfully"));
                return RedirectToAction("Login", "Authentication");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
