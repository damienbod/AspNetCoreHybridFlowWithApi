using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebHybridClient;
using WebMVCClient.Models;

namespace WebMVCClient.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public async System.Threading.Tasks.Task<IActionResult> Index()
        {
            var apiService = new ApiService();
            var result = await apiService.GetApiDataAsync();

            ViewData["data"] = result.ToString();
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
