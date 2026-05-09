namespace Sales_Management_System_API.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string htmlBody);
    }
}
