using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DeviceFlowWeb.Pages
{
    public class LoginModel : PageModel
    {
        private readonly DeviceFlowService _deviceFlowService;

        public LoginModel(DeviceFlowService deviceFlowService)
        {
            _deviceFlowService = deviceFlowService;
        }

        public async Task OnGetAsync()
        {
            var codes = await _deviceFlowService.BeginLogin();
        }
    }
}