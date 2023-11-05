using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace WebCodeFlowPkceClient.Pages;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    [BindProperty]
    public IEnumerable<Claim> Claims { get; set; } = Enumerable.Empty<Claim>();

    public void OnGet()
    {
        // var claims = User.Claims.ToList();
        Claims = User.Claims;
    }
}