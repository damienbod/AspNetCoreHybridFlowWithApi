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
    public class RequirePasswordCheckModel : BasePasswordCheck
    {
        public RequirePasswordCheckModel(UserManager<ApplicationUser> userManager): base(userManager)
        {

        }
        public async Task OnGetAsync()
        {
            // https://localhost:44327/UserCheck?returnUrl=/RequirePasswordCheck

            var passwordCheckOk = await ValidatePasswordCheck();
            if(!passwordCheckOk)
            {
                Redirect("/UserCheck?returnUrl=/DoUserChecks/RequirePasswordCheck");
            }

            var ok = "";
        }
    }
}
