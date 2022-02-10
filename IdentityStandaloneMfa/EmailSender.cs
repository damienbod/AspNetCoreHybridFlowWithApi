using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace IdentityStandaloneMfa;

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string message)
    {
        return Task.CompletedTask;
    }
}
