using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using DTech.Models.ViewModel;
using DTech.Models.EF;
using DTech.DAO;
using CloudinaryDotNet;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DTech.Controllers
{
    public class AuthenticationController (
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        CustomerDAO customerDAO,
        RoleDAO roleDAO,
        CartDAO cartDAO,
        CustomerAddressDAO customerAddressDAO
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

                if(newUser.Password.Length < 6)
                {
                    ModelState.AddModelError("Password", "Password must be at least 6 character.");
                    return View("Register", newUser);
                }

                if (newUser.ConfirmPassword != newUser.Password)
                {
                    ModelState.AddModelError("ConfirmPassword", "Confirm Password is not match.");
                    return View("Register", newUser);
                }

                ApplicationUser user = new ()
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
                ViewBag.EmailError = ModelState["email"]?.Errors.First().ErrorMessage ?? "Invalid Email Address";
                return View();
            }
            var user = await userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                string resetLink = Url.Action("ResetPassword", "Authentication", new { token }) ?? "URL Error";
            }
            ViewBag.SuccessMessage = "Please check your email to reset your password.";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
