namespace IdentityStandaloneUserCheck.Services;

public class EmailSender : IEmailSender
{
    public Task SendEmail(string email, string subject, string message, string toUsername)
    {
        throw new Exception("not implemented");
    }
}
