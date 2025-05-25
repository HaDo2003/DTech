namespace DTech.Library.Service.Email
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
