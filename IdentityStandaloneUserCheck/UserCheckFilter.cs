using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace IdentityStandaloneUserCheck
{
    public class UserCheckFilter : IAsyncPageFilter
    {
        public static string PasswordCheckedClaimType = "passwordChecked";
        public static string LastloginClaimType = "lastlogin";

        
        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                if (context.HttpContext.User.HasClaim(c => c.Type == PasswordCheckedClaimType))
                {
                    var lastloginClaimTypeClaim = context.HttpContext.User.FindFirst(LastloginClaimType);
                    var dateTimeLastLogin = DateTime.FromFileTimeUtc(Convert.ToInt64(lastloginClaimTypeClaim.Value));

                    var lastCheckedClaim = context.HttpContext.User.FindFirst(PasswordCheckedClaimType);
                    var dateTimeLastUserCheck = DateTime.FromFileTimeUtc(Convert.ToInt64(lastCheckedClaim.Value));
                    if (dateTimeLastLogin > dateTimeLastUserCheck)
                    {
                        context.Result = new RedirectToPageResult("/UserCheck", "?returnUrl=/DoUserChecks/RequirePasswordCheck");
                    } 
                    else if (DateTime.UtcNow.AddMinutes(-10.0) > dateTimeLastUserCheck)
                    {
                        context.Result = new RedirectToPageResult("/UserCheck", "?returnUrl=/DoUserChecks/RequirePasswordCheck");
                    }
                    else
                    {
                        await next.Invoke();
                    }
                }   
            }
            else
            {
                await next.Invoke();
            }
        }

        public async Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {

        }
    }
}
