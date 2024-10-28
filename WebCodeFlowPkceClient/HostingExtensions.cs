using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.JsonWebTokens;
using Serilog;

namespace WebCodeFlowPkceClient;

internal static class HostingExtensions
{
    private static IWebHostEnvironment? _env;

    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;
        _env = builder.Environment;

        services.AddTransient<IClaimsTransformation, MyClaimsTransformation>();

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
        .AddCookie()
        .AddOpenIdConnect(options =>
        {
            options.SignInScheme = "Cookies";
            options.Authority = "https://localhost:44352";
            options.RequireHttpsMetadata = true;
            options.ClientId = "codeflowpkceclient";
            options.ClientSecret = "codeflow_pkce_client_secret";
            options.ResponseType = "code";
            options.UsePkce = true;
            options.Scope.Add("profile");
            options.Scope.Add("offline_access");
            options.SaveTokens = true;
            options.GetClaimsFromUserInfoEndpoint = true;
            options.ClaimActions.MapUniqueJsonKey("preferred_username", "preferred_username");
            options.ClaimActions.MapUniqueJsonKey("gender", "gender");
            options.MapInboundClaims = false;
            //options.TokenValidationParameters = new TokenValidationParameters
            //{
            //    NameClaimType = "email",
            //    //RoleClaimType = "Role",   
            //};
        });

        services.AddAuthorization();
        services.AddRazorPages();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        //IdentityModelEventSource.ShowPII = true;
        JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

        app.UseSerilogRequestLogging();

        if (_env!.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();

        return app;
    }
}
