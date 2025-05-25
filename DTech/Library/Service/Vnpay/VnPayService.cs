using DTech.Models.EF;
using DTech.Models.Vnpay;

namespace DTech.Library.Service.Vnpay
{
    public class VnPayService(IConfiguration configuration) : IVnPayService
    {
        public string CreatePaymentUrl(PaymentInformationModel model, string? clientIp)
        {
            var timeZoneId = configuration["TimeZoneId"];
            if (string.IsNullOrEmpty(timeZoneId))
            {
                throw new ArgumentException("TimeZoneId configuration value is missing or null.");
            }

            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var pay = new VnPayLibrary();
            var urlCallBack = configuration["Vnpay:PaymentBackReturnUrl"];
            var txnRef = !string.IsNullOrEmpty(model.TxnRef) ? model.TxnRef : DateTime.Now.Ticks.ToString();

            // Ensure all configuration values are checked for null or empty before adding them
            AddRequestDataSafely(pay, "vnp_Version", configuration["Vnpay:Version"]);
            AddRequestDataSafely(pay, "vnp_Command", configuration["Vnpay:Command"]);
            AddRequestDataSafely(pay, "vnp_TmnCode", configuration["Vnpay:TmnCode"]);

            if (!model.Amount.HasValue)
            {
                throw new ArgumentException("PaymentInformationModel.Amount cannot be null.");
            }
            AddRequestDataSafely(pay, "vnp_Amount", ((long)model.Amount.Value * 100).ToString());

            AddRequestDataSafely(pay, "vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            AddRequestDataSafely(pay, "vnp_CurrCode", configuration["Vnpay:Currency"]);
            AddRequestDataSafely(pay, "vnp_IpAddr", clientIp ?? "");
            AddRequestDataSafely(pay, "vnp_Locale", configuration["Vnpay:Locale"]);
            AddRequestDataSafely(pay, "vnp_OrderInfo", $"{model.Name} {model.OrderDescription} {model.Amount}");
            AddRequestDataSafely(pay, "vnp_OrderType", model.OrderType);
            AddRequestDataSafely(pay, "vnp_ReturnUrl", urlCallBack);
            AddRequestDataSafely(pay, "vnp_TxnRef", txnRef);

            var baseUrl = configuration["Vnpay:BaseUrl"];
            var hashSecret = configuration["Vnpay:HashSecret"];
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new ArgumentException("Configuration value for 'Vnpay:BaseUrl' is missing or null.");
            }
            if (string.IsNullOrEmpty(hashSecret))
            {
                throw new ArgumentException("Configuration value for 'Vnpay:HashSecret' is missing or null.");
            }

            var paymentUrl = pay.CreateRequestUrl(baseUrl, hashSecret);
            Console.WriteLine("Payment URL: " + paymentUrl);
            return paymentUrl;
        }

        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var hashSecret = configuration["Vnpay:HashSecret"];
            if (string.IsNullOrEmpty(hashSecret))
            {
                throw new ArgumentException("Configuration value for 'Vnpay:HashSecret' is missing or null.");
            }

            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, hashSecret);

            return response;
        }

        private void AddRequestDataSafely(VnPayLibrary pay, string key, string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"Configuration value for '{key}' is missing or null.");
            }
            pay.AddRequestData(key, value);
        }
    }
}
