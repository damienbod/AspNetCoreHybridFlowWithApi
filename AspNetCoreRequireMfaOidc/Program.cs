using AspNetCoreRequireMfaOidc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
});

builder.Services.AddSingleton<IAuthorizationHandler, RequireMfaHandler>();

builder.Services.AddAuthentication(options =>
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
    options.ClientId = "AspNetCoreRequireMfaOidc";
    options.ClientSecret = "AspNetCoreRequireMfaOidcSecret";
    options.ResponseType = "code id_token";
    options.Scope.Add("profile");
    options.Scope.Add("offline_access");
    options.SaveTokens = true;
    options.Events = new OpenIdConnectEvents
    {
        OnRedirectToIdentityProvider = context =>
        {
            context.ProtocolMessage.SetParameter("acr_values", Amr.Mfa);

            return Task.FromResult(0);
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireMfa", policyIsAdminRequirement =>
    {
        policyIsAdminRequirement.Requirements.Add(new RequireMfa());
    });
});

builder.Services.AddRazorPages();

var app = builder.Build();

app.UseCookiePolicy();

//IdentityModelEventSource.ShowPII = true;
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

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
