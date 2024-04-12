using System.Net;
using ShopDev.ApplicationBase.Localization;
using ShopDev.Constants.ErrorCodes;
using ShopDev.Utils.Net.Request;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using OpenIddict.Abstractions;

namespace ShopDev.IdentityServerBase.Middlewares
{
    public class CheckAuthorizationTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMapErrorCode _mapErrorCode;

        public CheckAuthorizationTokenMiddleware(RequestDelegate next, IMapErrorCode mapErrorCode)
        {
            _next = next;
            _mapErrorCode = mapErrorCode;
        }

        public async Task InvokeAsync(HttpContext context, IOpenIddictTokenManager tokenManager)
        {
            var authorizationId = context
                .User.Claims.FirstOrDefault(c => c.Type == "oi_au_id")
                ?.Value;
            if (authorizationId is not null)
            {
                var tokens = tokenManager.FindByAuthorizationIdAsync(authorizationId);
                await foreach (var token in tokens)
                {
                    if (
                        await tokenManager.HasStatusAsync(
                            token,
                            OpenIddictConstants.Statuses.Revoked
                        )
                    )
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        await context.Response.WriteAsJsonAsync(
                            new ApiResponse(
                                StatusCode.Error,
                                string.Empty,
                                ErrorCode.Unauthorized,
                                _mapErrorCode.GetErrorMessage(ErrorCode.Unauthorized)
                            )
                        );
                        return;
                    }
                }
            }
            await _next(context);
        }
    }

    public static class CheckAuthorizationTokenMiddlewareExtensions
    {
        public static IApplicationBuilder UseCheckAuthorizationToken(
            this IApplicationBuilder builder
        )
        {
            return builder.UseMiddleware<CheckAuthorizationTokenMiddleware>();
        }
    }
}
