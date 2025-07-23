using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using WebAppMvc.Models;

namespace WebAppMvc.Services
{
    public class MailService : IEmailSender
    {
        private readonly MailSettings _mailSettings;
        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }
        public async Task SendEmailAsync(string emailto, string subject, string htmlMessage)
        {
            var Email = new MimeMessage();
            Email.Sender = MailboxAddress.Parse(_mailSettings.SenderEMail);
            Email.Sender.Name = _mailSettings.DisplayName;
            Email.To.Add(MailboxAddress.Parse(emailto));
            Email.Subject = subject;
            Email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };
            var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.SenderEMail, _mailSettings.Password);
            await smtp.SendAsync(Email);
            smtp.Disconnect(true);
        }        
    }   
}
