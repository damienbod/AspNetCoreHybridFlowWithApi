using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WebHybridClient
{
    public class Startup
    {
        private string stsServer = "";
        private readonly IWebHostEnvironment _environment;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            _environment = webHostEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<ApiService>();
            services.AddSingleton<ApiTokenInMemoryClient>();
            services.AddSingleton<ApiTokenCacheClient>();
            
            services.AddHttpClient();
            services.Configure<AuthConfigurations>(Configuration.GetSection("AuthConfigurations"));

            var authConfigurations = Configuration.GetSection("AuthConfigurations");
            stsServer = authConfigurations["StsServer"];

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
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            app.UseSecurityHeaders(
                SecurityHeadersDefinitions.GetHeaderPolicyCollection(env.IsDevelopment()));

            if (_environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
