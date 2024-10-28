using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Logging;
using NetEscapades.AspNetCore.SecurityHeaders.Infrastructure;
using Serilog;

namespace WebHybridClient;

internal static class HostingExtensions
{
    private static IWebHostEnvironment? _env;

    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;
        _env = builder.Environment;

        services.AddSecurityHeaderPolicies()
            .SetPolicySelector((PolicySelectorContext ctx) =>
            {
                return SecurityHeadersDefinitions
                    .GetHeaderPolicyCollection(_env!.IsDevelopment());
            });

        services.AddTransient<ApiService>();
        services.AddSingleton<ApiTokenInMemoryClient>();
        services.AddSingleton<ApiTokenCacheClient>();

        services.AddHttpClient();
        services.Configure<AuthConfigurations>(configuration.GetSection("AuthConfigurations"));

        var authConfigurations = configuration.GetSection("AuthConfigurations");
        var stsServer = authConfigurations["StsServer"];

        services.AddDistributedMemoryCache();

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
        .AddCookie()
        .AddOpenIdConnect(options =>
        {
            options.SignInScheme = "Cookies";
            options.Authority = stsServer;
            options.RequireHttpsMetadata = true;
            options.ClientId = "hybridclient";
            options.ClientSecret = "hybrid_flow_secret";
            options.ResponseType = "code id_token";
            options.Scope.Add("scope_used_for_hybrid_flow");
            options.Scope.Add("profile");
            options.Scope.Add("offline_access");
            options.SaveTokens = true;
        });

        services.AddAuthorization();

        services.AddControllersWithViews();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        IdentityModelEventSource.ShowPII = true;
        JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

        app.UseSerilogRequestLogging();

        app.UseSecurityHeaders();

        if (_env!.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
        }

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}"
        );
        return app;
    }
}
