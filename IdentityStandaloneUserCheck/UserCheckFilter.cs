using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace IdentityStandaloneUserCheck
{
    public class UserCheckFilter : IAsyncPageFilter
    {
        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var claimType = "passwordChecked";
                if (context.HttpContext.User.HasClaim(c => c.Type == claimType))
                {
                    var lastChecked = context.HttpContext.User.FindFirst(claimType);
                    var dateTimeLastUserCheck = DateTime.FromFileTimeUtc(Convert.ToInt64(lastChecked.Value));
                    if (DateTime.UtcNow.AddMinutes(-10.0) > dateTimeLastUserCheck)
                    {
                        context.Result = new RedirectToPageResult("/UserCheck", "?returnUrl=/DoUserChecks/RequirePasswordCheck");
                    }
                }   
            }

            await next.Invoke();
        }

        public async Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {

        }
    }
}
