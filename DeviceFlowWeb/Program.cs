using DeviceFlowWeb;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;
var env = builder.Environment;

services.AddScoped<DeviceFlowService>();
services.AddHttpClient();
services.Configure<AuthConfigurations>(configuration.GetSection("AuthConfigurations"));

services.AddSession(options =>
{
    // Set a short timeout for easy testing.
    options.IdleTimeout = TimeSpan.FromSeconds(60);
    options.Cookie.HttpOnly = true;
});

services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

var authConfigurations = configuration.GetSection("AuthConfigurations");
var stsServer = authConfigurations["StsServer"];

services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie();

services.AddAuthorization();
services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

services.AddRazorPages();

var app = builder.Build();

app.UseSecurityHeaders(SecurityHeadersDefinitions
    .GetHeaderPolicyCollection(env.IsDevelopment()));

if (env.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseCookiePolicy();
app.UseSession();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
