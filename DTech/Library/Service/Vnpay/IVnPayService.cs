using DTech.Models.Vnpay;

namespace DTech.Library.Service.Vnpay
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentInformationModel model, string? clientIp);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);

    }
}
