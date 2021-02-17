using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace IdentityStandaloneUserCheck
{
    public class UserCheckFilter : ResultFilterAttribute
    {

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                if (!context.ActionDescriptor.DisplayName.Contains("UserCheck", StringComparison.OrdinalIgnoreCase))
                {
                    var claimType = "passwordChecked";
                    if (context.HttpContext.User.HasClaim(c => c.Type == claimType))
                    {
                        var lastChecked = context.HttpContext.User.FindFirst(claimType);
                        var dateTime = DateTime.FromFileTimeUtc(Convert.ToInt64(lastChecked.Value));
                        if(DateTime.UtcNow > dateTime.AddMinutes(-10.0))
                        {
                            context.Result = new RedirectToPageResult("/UserCheck?returnUrl=/RequirePasswordCheck");
                        }

                        await next.Invoke();
                    }
                    
                    context.Result = new RedirectToPageResult("/UserCheck?returnUrl=/RequirePasswordCheck");
                }
            }
            else
            {
                await next.Invoke();
            }
        }
    }
}
