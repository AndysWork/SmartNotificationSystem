using NotificationService.Services.Interface;
using SendGrid.Helpers.Mail;
using SendGrid;

namespace NotificationService.Services.Implementation
{
    public class EmailService(IConfiguration configuration) : IEmailService
    {
        private readonly IConfiguration _configuration = configuration;

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var sendgridApiKey = _configuration["SendGridSettings:ApiKey"];
            var senderEmail = _configuration["SendGridSettings:FromEmail"];
            var senderName = _configuration["SendGridSettings:FromName"];

            var client = new SendGridClient(sendgridApiKey);
            var from = new EmailAddress(senderEmail, senderName);
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, body, body);
            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                // Handle error
                var responseBody = await response.Body.ReadAsStringAsync();
                throw new Exception($"Failed to send email: {response.StatusCode}, {responseBody}");
            }

        }
    }
}
