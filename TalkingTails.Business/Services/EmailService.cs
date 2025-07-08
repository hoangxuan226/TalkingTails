using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using TalkingTails.Business.Interfaces;
using TalkingTails.Business.Models.Setting;
using TalkingTails.Business.Templates;

namespace TalkingTails.Business.Services
{
    public class EmailService(IOptions<EmailSettings> emailOptions) : IEmailService
    {
        private readonly EmailSettings _emailSettings = emailOptions.Value;

        public async Task SendPasswordResetEmailAsync(string toEmail, string resetToken, string userFullName)
        {
            var subject = "Đặt lại mật khẩu - TalkingTails";
            var resetLink =
                $"{_emailSettings.ClientUrl}/reset-password?token={Uri.EscapeDataString(resetToken)}&email={Uri.EscapeDataString(toEmail)}";

            var displayName = ExtractUserNameFromEmailIfNeeded(userFullName);
            var body = PasswordResetEmail.GetTemplate(displayName, resetLink);

            await SendEmailAsync(toEmail, subject, body);
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using var client = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort);
            client.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
            client.EnableSsl = _emailSettings.EnableSsl;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);
            await client.SendMailAsync(mailMessage);
        }

        private static string ExtractUserNameFromEmailIfNeeded(string userFullName)
        {
            // Check if the userFullName contains '@' (looks like an email)
            if (string.IsNullOrWhiteSpace(userFullName) || !userFullName.Contains('@'))
            {
                return userFullName; // Return as-is if not an email
            }

            // Extract the part before '@'
            var atIndex = userFullName.IndexOf('@');
            return atIndex > 0 ? userFullName[..atIndex] : userFullName;
        }
    }
}