using IdentityStandaloneUserCheck.Data;
using IdentityStandaloneUserCheck.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
       options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(
    options => options.SignIn.RequireConfirmedAccount = false)
    .AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddRazorPages();
var app = builder.Build();

//IdentityModelEventSource.ShowPII = true;
//JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
