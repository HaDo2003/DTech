using DTech.Models.Vnpay;

namespace DTech.Library.Service.Vnpay
{
    public class VnPayService(IConfiguration configuration) : IVnPayService
    {
        public string CreatePaymentUrl(PaymentInformationModel model, string? clientIp)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new VnPayLibrary();
            var urlCallBack = configuration["Vnpay:PaymentBackReturnUrl"];

            pay.AddRequestData("vnp_Version", configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", ((long)model.Amount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", configuration["Vnpay:Currency"]);
            pay.AddRequestData("vnp_IpAddr", clientIp ?? "");
            pay.AddRequestData("vnp_Locale", configuration["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"{model.Name} {model.OrderDescription} {model.Amount}");
            pay.AddRequestData("vnp_OrderType", model.OrderType);
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            pay.AddRequestData("vnp_TxnRef", tick);

            var paymentUrl =
                pay.CreateRequestUrl(configuration["Vnpay:BaseUrl"], configuration["Vnpay:HashSecret"]);
            Console.WriteLine("Payment URL: " + paymentUrl);
            return paymentUrl;
        }

        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, configuration["Vnpay:HashSecret"]);

            return response;
        }
    }
}
