using System.Collections.Generic;
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

            /// TODO Add the cliams from the token to the cookie
            /// 
            if (tokenresponse.IsError)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            var claims = new List<Claim>
                {
                    //new Claim(ClaimTypes.Name, user.Email),
                    //new Claim("FullName", user.FullName),
                    new Claim(ClaimTypes.Role, "Administrator"),
                };

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

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            
            return Redirect("/Index");
        }
    }
}