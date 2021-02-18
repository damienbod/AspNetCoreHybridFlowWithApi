using System;
using System.Threading.Tasks;
using IdentityStandaloneUserCheck.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityStandaloneUserCheck.Pages
{
    public class PasswordVerificationBase : PageModel
    {
        public static string PasswordCheckedClaimType = "passwordChecked";
        public static string LastLoginClaimType = "lastlogin";

        private readonly UserManager<ApplicationUser> _userManager;

        public PasswordVerificationBase(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> ValidatePasswordVerification()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.HasClaim(c => c.Type == PasswordCheckedClaimType))
                {
                    var user = await _userManager.FindByEmailAsync(User.Identity.Name);

                    var lastLogin = DateTime.FromFileTimeUtc(Convert.ToInt64(user.LastLogin));

                    var lastPasswordVerificationClaim = User.FindFirst(PasswordCheckedClaimType);
                    var lastPasswordVerification = DateTime.FromFileTimeUtc(Convert.ToInt64(lastPasswordVerificationClaim.Value));
                    if (lastLogin > lastPasswordVerification)
                    {
                        return false;
                    }
                    else if (DateTime.UtcNow.AddMinutes(-10.0) > lastPasswordVerification)
                    {
                        return false;
                    }

                    return true;
                }
            }

            return false;

        }
    }
}
