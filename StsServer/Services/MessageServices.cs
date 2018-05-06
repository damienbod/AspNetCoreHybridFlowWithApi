using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace StsServer.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender
    {
        private readonly ILogger<AuthMessageSender> _logger;

        public AuthMessageSender(ILogger<AuthMessageSender> logger)
        {
            _logger = logger;
        }
        public Task SendEmailAsync(string email, string subject, string message)
        {
            // Plug in your email service here to send an email.
            _logger.LogInformation("Email: {email}, Subject: {subject}, Message: {message}", email, subject, message);
            return Task.FromResult(0);
        }
    }
}
