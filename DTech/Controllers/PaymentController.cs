using DTech.Library.Service.Vnpay;
using DTech.Models.Vnpay;
using Microsoft.AspNetCore.Mvc;

namespace DTech.Controllers
{
    public class PaymentController(IVnPayService vnPayService) : Controller
    {
        //public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        //{
        //    var url = vnPayService.CreatePaymentUrl(model, HttpContext);

        //    return Redirect(url);
        //}
    }
}
