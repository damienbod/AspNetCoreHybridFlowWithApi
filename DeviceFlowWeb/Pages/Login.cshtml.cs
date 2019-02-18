using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DeviceFlowWeb.Pages
{
    public class LoginModel : PageModel
    {
        private readonly DeviceFlowService _deviceFlowService;

        private DeviceAuthorizationResponse _deviceAuthorizationResponse;

        public string AuthenticatorUri { get; set; }

        public string UserCode { get; set; }

        public LoginModel(DeviceFlowService deviceFlowService)
        {
            _deviceFlowService = deviceFlowService;
        }

        public async Task OnGetAsync()
        {
            _deviceAuthorizationResponse = await _deviceFlowService.BeginLogin();
            AuthenticatorUri = _deviceAuthorizationResponse.VerificationUri;
            UserCode = _deviceAuthorizationResponse.UserCode;

        }

        public async Task<IActionResult> OnPostAsync()
        {
            var tokenresponse = await _deviceFlowService.RequestTokenAsync(_deviceAuthorizationResponse);

            return Page();
        }
    }
}