using System.Threading.Tasks;
using IdentityModel.Client;
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

            return Page();
        }
    }
}