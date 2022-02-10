using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(IdentityStandaloneUserCheck.Areas.Identity.IdentityHostingStartup))]
namespace IdentityStandaloneUserCheck.Areas.Identity;

public class IdentityHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
        });
    }
}