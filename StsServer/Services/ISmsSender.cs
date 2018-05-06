using System.Threading.Tasks;

namespace IdentityServerWithAspNetIdentity.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
