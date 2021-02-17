using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityStandaloneUserCheck.Pages
{
    [UserCheckFilter]
    public class RequirePasswordCheckModel : PageModel
    {
        public void OnGet()
        {
            // https://localhost:44327/UserCheck?returnUrl=/RequirePasswordCheck


            // check for claim
            // redirect if missing
            // continue if ok
        }
    }
}
