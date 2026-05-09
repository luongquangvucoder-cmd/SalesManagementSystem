using MailKit.Net.Smtp;
using MimeKit;
using Sales_Management_System_API.Services.Interfaces;

namespace Sales_Management_System_API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlBody)
        {
            var smtpSettings = _configuration.GetSection("EmailSettings");

            var smtpServer = smtpSettings["SmtpServer"]
                ?? throw new Exception("SMTP Server missing");

            var senderEmail = smtpSettings["SenderEmail"]
                ?? throw new Exception("Sender email missing");

            var senderPassword = smtpSettings["SenderPassword"]
                ?? throw new Exception("Sender password missing");

            var senderName = smtpSettings["SenderName"];

            var smtpPort = int.Parse(smtpSettings["SmtpPort"]!);

            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(senderName, senderEmail));

            message.To.Add(MailboxAddress.Parse(email));

            message.Subject = subject;

            message.Body = new BodyBuilder
            {
                HtmlBody = htmlBody
            }.ToMessageBody();

            using var client = new SmtpClient();

            await client.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(senderEmail, senderPassword);

            await client.SendAsync(message);

            await client.DisconnectAsync(true);
        }
    }
}