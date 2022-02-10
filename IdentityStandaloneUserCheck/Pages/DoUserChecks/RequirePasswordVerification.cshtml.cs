using System.Threading.Tasks;
using IdentityStandaloneUserCheck.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityStandaloneUserCheck.Pages;

public class RequirePasswordVerificationModel : PasswordVerificationBase
{
    public RequirePasswordVerificationModel(UserManager<ApplicationUser> userManager) : base(userManager)
    {}

    public async Task<IActionResult> OnGetAsync()
    {
        var passwordVerificationOk = await ValidatePasswordVerification();
        if (!passwordVerificationOk)
        {
            return RedirectToPage("/PasswordVerification", new { ReturnUrl = "/DoUserChecks/RequirePasswordVerification" });
        }

        return Page();
    }
}
