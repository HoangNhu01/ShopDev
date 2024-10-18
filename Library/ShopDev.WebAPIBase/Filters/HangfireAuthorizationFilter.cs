using ShopDev.Constants.Users;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using ShopDev.Constants.Domain.Auth.Authorization;

namespace ShopDev.WebAPIBase.Filters
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var cookie = context
                .GetHttpContext()
                .AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme)
                .Result;
            if (
                cookie.Succeeded
                && cookie.Principal.Claims.Any(e =>
                    e.Type == ClaimTypes.UserType && e.Value == UserTypes.SUPER_ADMIN.ToString()
                )
            )
            {
                return true;
            }
            context.GetHttpContext().Response.Redirect(AuthenticationPath.AuthenticateLogin);
            return true;
        }
    }
}
