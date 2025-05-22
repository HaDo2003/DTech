using DTech.DAO;
using DTech.Models.EF;
using DTech.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DTech.Controllers
{
    [Authorize]
    [Route("checkout")]
    public class CheckOutController(
        CartDAO cartDAO,
        ProductDAO productDAO,
        OrderDAO orderDAO,
        ShippingDAO shippingDAO,
        PaymentDAO paymentDAO,
        CustomerAddressDAO customerAddressDAO,
        PaymentMethodDAO paymentMethodDAO
    ) : Controller
    {
        public async Task<IActionResult> CheckOut()
        {
            var model = new CheckoutViewModel();
            ViewData["PaymentId"] = new SelectList(await paymentMethodDAO.GetListAsync(), "PaymentMethodId", "PaymentMethodId");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut(
            CheckoutViewModel model
        )
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Authentication");


            if (!ModelState.IsValid)
            {
                //ViewData["PaymentId"] = new SelectList(await paymentMethodDAO.GetListAsync(), "PaymentMethodId", "PaymentMethodId", order.Payment.PaymentMethodId);
                //return View(order);
            }

            //Create a new shipping record
            Shipping shipping = new()
            {
                DelivaryDate = DateOnly.FromDateTime(DateTime.Now),
            };
            await shippingDAO.AddAsync(shipping);

            //Create a new payment record
            Payment payment = new()
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                //Amount = order.FinalCost,
                Status = 1,
                CreatedBy = userId,
                CreateDate = DateTime.Now,
            };

            await paymentDAO.AddAsync(payment);

            //order.CustomerId = userId;
            //order.ShippingId = shipping.ShippingId;
            //order.PaymentId = payment.PaymentId;

            //var result = await orderDAO.AddAsync(order);
            //if (result)
            //{
            //    return View("Success");
            //}
            return View();
        }
    }
}
