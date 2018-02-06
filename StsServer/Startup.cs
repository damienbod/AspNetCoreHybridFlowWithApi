using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StsServer.Data;
using StsServer.Models;
using StsServer.Services;
using QuickstartIdentityServer;
using IdentityServer4.Services;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace StsServer
{
    public class Startup
    {
        private readonly IHostingEnvironment _environment;

        public Startup(IHostingEnvironment env)
        {
            //Log.Logger = new LoggerConfiguration()
            //    .MinimumLevel.Verbose()
            //    .Enrich.WithProperty("App", "StsServer")
            //    .Enrich.FromLogContext()
            //    .WriteTo.Seq("http://localhost:5341")
            //    .WriteTo.RollingFile("../Logs/StsServer")
            //    .CreateLogger();

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
				
            _environment = env;

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //var cert = new X509Certificate2(Path.Combine(_environment.ContentRootPath, "damienbodserver.pfx"), "");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddAuthentication();

            services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddMvc();

            services.AddTransient<IProfileService, IdentityWithAdditionalClaimsProfileService>();

            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            var authConfigurations = Configuration.GetSection("AuthConfigurations");

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients(authConfigurations))
                .AddAspNetIdentity<ApplicationUser>()
                .AddProfileService<IdentityWithAdditionalClaimsProfileService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            loggerFactory.AddSerilog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentityServer();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
