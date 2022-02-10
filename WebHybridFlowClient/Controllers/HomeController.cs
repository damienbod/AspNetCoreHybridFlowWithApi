using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebHybridClient;
using WebHybridClient.Models;

namespace WebHybridClient.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ApiService _apiService;

    public HomeController(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async System.Threading.Tasks.Task<IActionResult> Index()
    {
        var result = await _apiService.GetApiDataAsync();
        return View(result);
    }

    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}