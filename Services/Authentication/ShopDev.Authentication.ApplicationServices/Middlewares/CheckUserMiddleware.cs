using System.Net;
using ShopDev.ApplicationBase.Localization;
using ShopDev.Authentication.Infrastructure.Persistence;
using ShopDev.Constants.ErrorCodes;
using ShopDev.Constants.Users;
using ShopDev.Utils.Net.Request;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ShopDev.Authentication.ApplicationServices.Middlewares
{
    public class CheckUserMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMapErrorCode _mapErrorCode;

        public CheckUserMiddleware(RequestDelegate next, IMapErrorCode mapErrorCode)
        {
            _next = next;
            _mapErrorCode = mapErrorCode;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var claim = context.User.FindFirst(UserClaimTypes.UserId);
            var dbContext = context.RequestServices.GetRequiredService<AuthenticationDbContext>();

            if (claim != null)
            {
                int userId = int.Parse(claim.Value);
                var user = dbContext
                    .Users.Select(u => new { u.Id, u.Status })
                    .FirstOrDefault(u => u.Id == userId);
                if (user != null && user.Status != UserStatus.ACTIVE)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    await context.Response.WriteAsJsonAsync(
                        new ApiResponse(
                            StatusCode.Error,
                            string.Empty,
                            ErrorCode.UserIsDeactive,
                            _mapErrorCode.GetErrorMessage(ErrorCode.UserIsDeactive)
                        )
                    );
                    return;
                }
            }
            await _next(context);
        }
    }

    /// <summary>
    /// Extension check user middleware
    /// </summary>
    public static class CheckUserMiddlewareExtensions
    {
        public static IApplicationBuilder UseCheckUser(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CheckUserMiddleware>();
        }
    }
}
