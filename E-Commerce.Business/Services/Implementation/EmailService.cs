using E_Commerce.Business.Services.Interfaces;
using E_Commerce.Business.Configuration;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace E_Commerce.Business.Services.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using (var client = new SmtpClient(_settings.Server, _settings.Port))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(_settings.Username, _settings.Password);
                var mail = new MailMessage(_settings.FromEmail, toEmail, subject, body)
                {
                    IsBodyHtml = true
                };
                await client.SendMailAsync(mail);
            }
        }
    }
}
