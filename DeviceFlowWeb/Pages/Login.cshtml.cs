using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;

namespace DeviceFlowWeb.Pages
{
    public class LoginModel : PageModel
    {
        private readonly DeviceFlowService _deviceFlowService;

        public string AuthenticatorUri { get; set; }

        public string UserCode { get; set; }

        public LoginModel(DeviceFlowService deviceFlowService)
        {
            _deviceFlowService = deviceFlowService;
        }

        public async Task OnGetAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var deviceAuthorizationResponse = await _deviceFlowService.BeginLogin();
            AuthenticatorUri = deviceAuthorizationResponse.VerificationUri;
            UserCode = deviceAuthorizationResponse.UserCode;

            // Requires: using Microsoft.AspNetCore.Http;
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("DeviceCode")))
            {
                HttpContext.Session.SetString("DeviceCode", deviceAuthorizationResponse.DeviceCode);
                HttpContext.Session.SetInt32("Interval", deviceAuthorizationResponse.Interval);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var deviceCode = HttpContext.Session.GetString("DeviceCode");
            var interval = HttpContext.Session.GetInt32("Interval");

            if(interval.GetValueOrDefault() <= 0)
            {
                interval = 1;
            }

            var tokenresponse = await _deviceFlowService.RequestTokenAsync(deviceCode, interval.Value);

            if (tokenresponse.IsError)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            var claims = GetClaims(tokenresponse.IdentityToken);

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                //IsPersistent = true,
                // Whether the authentication session is persisted across 
                // multiple requests. Required when setting the 
                // ExpireTimeSpan option of CookieAuthenticationOptions 
                // set with AddCookie. Also required when setting 
                // ExpiresUtc.

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };
            var accessToken = new AuthenticationToken
            {
                Name = "access_token",
                Value = tokenresponse.AccessToken
            };

            var idToken = new AuthenticationToken
            {
                Name = "id_token",
                Value = tokenresponse.IdentityToken
            };

            authProperties.StoreTokens(new List<AuthenticationToken>
            {
                accessToken, idToken
            });

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return Redirect("/Index");
        }

        public IEnumerable<Claim> GetClaims(string token)
        {
            var validJwt = new JwtSecurityToken(token);
            return validJwt.Claims;
        }
    }
}