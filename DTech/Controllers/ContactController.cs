using DTech.DAO;
using DTech.Library.Service.BackgroundTask;
using DTech.Library.Service.Email;
using DTech.Models.EF;
using DTech.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DTech.Controllers
{
    [Route("contact")]
    public class ContactController(
        FeedbackDAO feedbackDAO,
        CustomerDAO customerDAO,
        IEmailService emailService,
        IBackgroundTaskQueue taskQueue
    ) : Controller
    {
        [HttpGet("")]
        public async Task<IActionResult> Contact()
        {
            //Check if user is authenticated
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = new ContactViewModel();
            if (userId != null)
            {
                //Get user information
                var customer = await customerDAO.GetByIdAsync(userId);
                if (customer != null)
                {
                    model.Name = customer.FullName ?? string.Empty;
                    model.Email = customer.Email ?? string.Empty;
                    model.PhoneNumber = customer.PhoneNumber ?? string.Empty;
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Create new feedback
                var feedback = new Feedback
                {
                    Name = model.Name,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Detail = model.Message,
                    Fbdate = DateTime.Now
                };
                //Save feedback to database
                var result = await feedbackDAO.AddAsync(feedback);
                if (result)
                {
                    if (!string.IsNullOrEmpty(model.Email))
                    {
                        string subject = "Thank you for contacting DTech!";
                        string body = $@"
                        <html>
                            <body style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
                                <div style='max-width: 600px; margin: auto; background: white; padding: 20px; border-radius: 10px; box-shadow: 0 2px 5px rgba(0,0,0,0.1);'>
                                    <h2 style='color: #007bff;'>Hi {model.Name},</h2>
                                    <p>Thank you for reaching out to <strong>DTech</strong>!</p>
                                    <p>We've received your message and will get back to you as soon as possible.</p>
                                    <p style='margin-top: 20px;'>Best regards,<br/>The DTech Team</p>
                                </div>
                            </body>
                        </html>";

                        // Run in background
                        taskQueue.QueueBackgroundWorkItem(async token =>
                        {
                            await emailService.SendEmailAsync(model.Email, subject, body);
                        });
                    }
                    TempData["SuccessMessage"] = "Your message has been sent successfully!";
                    return RedirectToAction("Contact");
                }
                else
                {
                    ViewData["ErrorMessage"] = "Oops! Something went wrong. Please check your input and try again.";
                }
            }
            return View(model);
        }
    }
}
