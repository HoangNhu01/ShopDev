using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ShopDev.ApplicationBase.Localization;
using ShopDev.Authentication.Infrastructure.Persistence;
using ShopDev.Constants.ErrorCodes;
using ShopDev.Constants.Users;
using ShopDev.Utils.Net.Request;

namespace ShopDev.Authentication.ApplicationServices.Middlewares
{
    public class VerifyClientStatusMiddleware(RequestDelegate next, IMapErrorCode mapErrorCode)
    {
        private readonly RequestDelegate _next = next;
        private readonly IMapErrorCode _mapErrorCode = mapErrorCode;

        public async Task InvokeAsync(HttpContext context)
        {
            var claim = context.User.FindFirst(UserClaimTypes.UserId);
            var dbContext = context.RequestServices.GetRequiredService<AuthenticationDbContext>();

            if (claim is not null)
            {
                int userId = int.Parse(claim.Value);
                var user = await dbContext.Users.FindAsync(userId);
                if (user is not null && user.Status != UserStatus.ACTIVE)
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

    public static class VerifyClientStatusMiddlewareExtensions
    {
        public static IApplicationBuilder VerifyClientStatus(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<VerifyClientStatusMiddleware>();
        }
    }
}
