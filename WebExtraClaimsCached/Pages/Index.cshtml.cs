using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebExtraClaimsCached.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            var ss = User.Identity.Name;
        }
    }
}