[assembly: HostingStartup(typeof(IdentityStandaloneMfa.Areas.Identity.IdentityHostingStartup))]
namespace IdentityStandaloneMfa.Areas.Identity;

public class IdentityHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
        });
    }
}