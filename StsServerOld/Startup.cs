using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using IdentityServer4.Services;
using StsServerIdentity.Models;
using StsServerIdentity.Data;
using StsServerIdentity.Resources;
using StsServerIdentity.Services;
using StsServerIdentity.Filters;
using StsServerIdentity.Services.Certificate;
using Serilog;
using Microsoft.AspNetCore.Http;
using Fido2NetLib;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Logging;

namespace StsServerIdentity;

public class Startup
{
    private string _clientId = "xxxxxx";
    private string _clientSecret = "xxxxx";
    private IConfiguration _configuration { get; }
    private IWebHostEnvironment _environment { get; }

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        _configuration = configuration;
        _environment = env;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<CookiePolicyOptions>(options =>
        {
            options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
            options.OnAppendCookie = cookieContext =>
                CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            options.OnDeleteCookie = cookieContext =>
                CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
        });

        _clientId = _configuration["MicrosoftClientId"];
        _clientSecret = _configuration["MircosoftClientSecret"];
        var authConfigurations = _configuration.GetSection("AuthConfigurations");
        var useLocalCertStore = Convert.ToBoolean(_configuration["UseLocalCertStore"]);
        var certificateThumbprint = _configuration["CertificateThumbprint"];

        var x509Certificate2Certs = GetCertificates(_environment, _configuration)
            .GetAwaiter().GetResult();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")));

        services.Configure<AuthConfigurations>(_configuration.GetSection("AuthConfigurations"));
        services.Configure<EmailSettings>(_configuration.GetSection("EmailSettings"));
        services.AddTransient<IProfileService, IdentityWithAdditionalClaimsProfileService>();
        services.AddTransient<IEmailSender, EmailSender>();
        AddLocalizationConfigurations(services);
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddErrorDescriber<StsIdentityErrorDescriber>()
            .AddDefaultTokenProviders()
            .AddTokenProvider<Fifo2UserTwoFactorTokenProvider>("FIDO2");

        if (_clientId != null)
        {
            services.AddAuthentication()
                .AddOpenIdConnect("Azure AD / Microsoft", "Azure AD / Microsoft", options => // Microsoft common
                {
                    //  https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration
                    options.ClientId = _clientId;
                    options.ClientSecret = _clientSecret;
                    options.SignInScheme = "Identity.External";
                    options.RemoteAuthenticationTimeout = TimeSpan.FromSeconds(30);
                    options.Authority = "https://login.microsoftonline.com/common/v2.0/";
                    options.ResponseType = "code";
                    options.UsePkce = false; // live does not support this yet
                    options.Scope.Add("profile");
                    options.Scope.Add("email");
                    options.ClaimActions.MapUniqueJsonKey("preferred_username", "preferred_username");
                    options.ClaimActions.MapAll(); // ClaimActions.MapUniqueJsonKey("amr", "amr");
                    //options.ClaimActions.Remove("amr");
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        NameClaimType = "email",
                    };
                    options.CallbackPath = "/signin-microsoft";
                    options.Prompt = "login"; // login, consent
                    options.Events = new OpenIdConnectEvents
                    {
                        OnRedirectToIdentityProvider = context =>
                        {
                            context.ProtocolMessage.SetParameter("acr_values", "mfa");

                            return Task.FromResult(0);
                        }
                    };
                });
        }
        else
        {
            services.AddAuthentication();
        }

        services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new SecurityHeadersAttribute());
            })
            .AddViewLocalization()
            .AddDataAnnotationsLocalization(options =>
            {
                options.DataAnnotationLocalizerProvider = (type, factory) =>
                {
                    var assemblyName = new AssemblyName(typeof(SharedResource).GetTypeInfo().Assembly.FullName);
                    return factory.Create("SharedResource", assemblyName.Name);
                };
            })
            .AddNewtonsoftJson();

        services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
            .AddSigningCredential(x509Certificate2Certs.ActiveCertificate)
            .AddInMemoryIdentityResources(Config.GetIdentityResources())
            .AddInMemoryApiResources(Config.GetApiResources())
            .AddInMemoryApiScopes(Config.GetApiScopes())
            .AddInMemoryClients(Config.GetClients(authConfigurations))
            .AddAspNetIdentity<ApplicationUser>()
            .AddProfileService<IdentityWithAdditionalClaimsProfileService>();

        services.Configure<Fido2Configuration>(_configuration.GetSection("fido2"));
        services.Configure<Fido2MdsConfiguration>(_configuration.GetSection("fido2mds"));
        services.AddScoped<Fido2Storage>();
        // Adds a default in-memory implementation of IDistributedCache.
        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(2);
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.None;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSecurityHeaders(
            SecurityHeadersDefinitions.GetHeaderPolicyCollection(env.IsDevelopment()));

        IdentityModelEventSource.ShowPII = true;
        app.UseCookiePolicy();

        if (_environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
        }

        var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
        app.UseRequestLocalization(locOptions.Value);

        app.UseSerilogRequestLogging();

        app.UseStaticFiles();

        app.UseRouting();

        app.UseIdentityServer();
        app.UseAuthorization();

        app.UseSession();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });
    }

    private static async Task<(X509Certificate2 ActiveCertificate, X509Certificate2 SecondaryCertificate)> GetCertificates(IWebHostEnvironment environment, IConfiguration configuration)
    {
        var certificateConfiguration = new CertificateConfiguration
        {
            // Use an Azure key vault
            CertificateNameKeyVault = configuration["CertificateNameKeyVault"], //"StsCert",
            KeyVaultEndpoint = configuration["AzureKeyVaultEndpoint"], // "https://damienbod.vault.azure.net"

            // Use a local store with thumbprint
            //UseLocalCertStore = Convert.ToBoolean(configuration["UseLocalCertStore"]),
            //CertificateThumbprint = configuration["CertificateThumbprint"],

            // development certificate
            DevelopmentCertificatePfx = Path.Combine(environment.ContentRootPath, "sts_dev_cert.pfx"),
            DevelopmentCertificatePassword = "1234" //configuration["DevelopmentCertificatePassword"] //"1234",
        };

        (X509Certificate2 ActiveCertificate, X509Certificate2 SecondaryCertificate) certs = await CertificateService.GetCertificates(
            certificateConfiguration).ConfigureAwait(false);

        return certs;
    }

    private static void AddLocalizationConfigurations(IServiceCollection services)
    {
        services.AddSingleton<LocService>();
        services.AddLocalization(options => options.ResourcesPath = "Resources");

        services.Configure<RequestLocalizationOptions>(
            options =>
            {
                var supportedCultures = new List<CultureInfo>
                    {
                        new CultureInfo("en-US"),
                        new CultureInfo("de-DE"),
                        new CultureInfo("de-CH"),
                        new CultureInfo("it-IT"),
                        new CultureInfo("gsw-CH"),
                        new CultureInfo("fr-FR"),
                        new CultureInfo("zh-Hans"),
                        new CultureInfo("ga-IE"),
                        new CultureInfo("es-MX")
                    };

                options.DefaultRequestCulture = new RequestCulture(culture: "de-DE", uiCulture: "de-DE");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                var providerQuery = new LocalizationQueryProvider
                {
                    QueryParameterName = "ui_locales"
                };

                options.RequestCultureProviders.Insert(0, providerQuery);
            });
    }

    private static void CheckSameSite(HttpContext httpContext, CookieOptions options)
    {
        if (options.SameSite == SameSiteMode.None)
        {
            var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
            if (DisallowsSameSiteNone(userAgent))
            {
                // For .NET Core < 3.1 set SameSite = (SameSiteMode)(-1)
                options.SameSite = SameSiteMode.Unspecified;
            }
        }
    }

    private static bool DisallowsSameSiteNone(string userAgent)
    {
        // Cover all iOS based browsers here. This includes:
        // - Safari on iOS 12 for iPhone, iPod Touch, iPad
        // - WkWebview on iOS 12 for iPhone, iPod Touch, iPad
        // - Chrome on iOS 12 for iPhone, iPod Touch, iPad
        // All of which are broken by SameSite=None, because they use the iOS networking stack
        if (userAgent.Contains("CPU iPhone OS 12") || userAgent.Contains("iPad; CPU OS 12"))
        {
            return true;
        }

        // Cover Mac OS X based browsers that use the Mac OS networking stack. This includes:
        // - Safari on Mac OS X.
        // This does not include:
        // - Chrome on Mac OS X
        // Because they do not use the Mac OS networking stack.
        if (userAgent.Contains("Macintosh; Intel Mac OS X 10_14") &&
            userAgent.Contains("Version/") && userAgent.Contains("Safari"))
        {
            return true;
        }

        // Cover Chrome 50-69, because some versions are broken by SameSite=None,
        // and none in this range require it.
        // Note: this covers some pre-Chromium Edge versions,
        // but pre-Chromium Edge does not require SameSite=None.
        if (userAgent.Contains("Chrome/5") || userAgent.Contains("Chrome/6"))
        {
            return true;
        }

        return false;
    }
}