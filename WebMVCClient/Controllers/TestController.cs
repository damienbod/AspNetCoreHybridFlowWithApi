using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebMVCClient.Controllers
{
    [Route("[controller]")]
    public class StatusController : Controller
    {
        [Route("test")]
        public IActionResult Test()
        {
            return View();
        }
    }
}
