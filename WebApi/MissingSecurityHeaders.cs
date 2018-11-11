using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi
{
    public class MissingSecurityHeaders : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var result = context.Result;
            if (result is ViewResult)
            {
                var featurePolicy ="accelerometer 'none'; camera 'none'; geolocation 'none'; gyroscope 'none'; magnetometer 'none'; microphone 'none'; payment 'none'; usb 'none'";

                if (!context.HttpContext.Response.Headers.ContainsKey("feature-policy"))
                {
                    context.HttpContext.Response.Headers.Add("feature-policy", featurePolicy);
                }

                var csp = "script-src 'self';style-src 'self' 'unsafe-inline';img-src 'self' data:;font-src 'self';form-action 'self';frame-ancestors 'self';block-all-mixed-content";
                // IE
                if (!context.HttpContext.Response.Headers.ContainsKey("X-Content-Security-Policy"))
                {
                    context.HttpContext.Response.Headers.Add("X-Content-Security-Policy", csp);
                }
            }
        }
    }
}
