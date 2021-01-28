using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
            HttpContext.Session.SetString("DeviceCode", string.Empty);

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var deviceAuthorizationResponse = await _deviceFlowService.RequestDeviceCode();
            AuthenticatorUri = deviceAuthorizationResponse.VerificationUri;
            UserCode = deviceAuthorizationResponse.UserCode;

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
                interval = 5;
            }

            var tokenresponse = await _deviceFlowService.RequestTokenAsync(deviceCode, interval.Value);

            if (tokenresponse.IsError)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            var claims = GetClaims(tokenresponse.IdentityToken);

            var claimsIdentity = new ClaimsIdentity(
                claims, 
                CookieAuthenticationDefaults.AuthenticationScheme, 
                "name", 
                "user");

            var authProperties = new AuthenticationProperties();

            // save the tokens in the cookie
            authProperties.StoreTokens(new List<AuthenticationToken>
            {
                new AuthenticationToken
                {
                    Name = "access_token",
                    Value = tokenresponse.AccessToken
                },
                new AuthenticationToken
                {
                    Name = "id_token",
                    Value = tokenresponse.IdentityToken
                }
            });

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return Redirect("/Index");
        }

        private IEnumerable<Claim> GetClaims(string token)
        {
            var validJwt = new JwtSecurityToken(token);
            return validJwt.Claims;
        }
    }
}