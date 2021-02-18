using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityStandaloneUserCheck.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityStandaloneUserCheck.Pages
{
    public class BasePasswordCheck : PageModel
    {
        public static string PasswordCheckedClaimType = "passwordChecked";
        public static string LastloginClaimType = "lastlogin";

        private readonly UserManager<ApplicationUser> _userManager;

        public BasePasswordCheck(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> ValidatePasswordCheck()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.HasClaim(c => c.Type == PasswordCheckedClaimType))
                {
                    var user = await _userManager.FindByEmailAsync(User.Identity.Name);

                    var dateTimeLastLogin = DateTime.FromFileTimeUtc(Convert.ToInt64(user.LastLogin));

                    var lastCheckedClaim = User.FindFirst(PasswordCheckedClaimType);
                    var dateTimeLastUserCheck = DateTime.FromFileTimeUtc(Convert.ToInt64(lastCheckedClaim.Value));
                    if (dateTimeLastLogin > dateTimeLastUserCheck)
                    {
                        return false;
                    }
                    else if (DateTime.UtcNow.AddMinutes(-10.0) > dateTimeLastUserCheck)
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
