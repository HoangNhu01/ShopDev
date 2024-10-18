using System.Net;
using ShopDev.Utils.Cache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using OpenIddict.Abstractions;
using ShopDev.ApplicationBase.Localization;
using ShopDev.Constants.Common.Cache;
using ShopDev.Constants.ErrorCodes;
using ShopDev.Constants.Users;
using ShopDev.Utils.Net.Request;

namespace ShopDev.IdentityServerBase.Middlewares
{
    public class VerifyAuthorizationTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMapErrorCode _mapErrorCode;
        private readonly IDistributedCache _distributedCache;

        public VerifyAuthorizationTokenMiddleware(
            RequestDelegate next,
            IMapErrorCode mapErrorCode,
            IDistributedCache distributedCache
        )
        {
            _next = next;
            _mapErrorCode = mapErrorCode;
            _distributedCache = distributedCache;
        }

        public async Task InvokeAsync(HttpContext context, IOpenIddictTokenManager tokenManager)
        {
            bool hasAllowAnonymous =
                context.GetEndpoint() != null
                && context
                    .GetEndpoint()!
                    .Metadata.Any(em => em.GetType() == typeof(AllowAnonymousAttribute));
            if (hasAllowAnonymous)
            {
                await _next(context);
                return;
            }

            var authorizationId = context
                .User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.AuthorizationId)
                ?.Value;
            if (authorizationId is not null)
            {
                var tokenId = context
                    .User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.TokenId)
                    ?.Value;

                if (tokenId == null)
                {
                    await _next(context);
                    return;
                }

                // Cache thông tin token
                string key = PrefixKeyCache.AuthTokenString(authorizationId, tokenId);
                var valueToken = await _distributedCache.GetValueAsync<bool?>(key);
                if (valueToken is null)
                {
                    var token = await tokenManager.FindByIdAsync(tokenId);
                    valueToken =
                        token is not null
                        && await tokenManager.HasStatusAsync(
                            token,
                            OpenIddictConstants.Statuses.Revoked
                        );
                    await _distributedCache.SetAsync(key, valueToken, CacheExpiredTimes.MEDIUM);
                }

                // Nếu token bị revoked
                if (valueToken == true)
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
            await _next(context);
        }
    }

    public static class VerifyAuthorizationTokenMiddlewareExtensions
    {
        public static IApplicationBuilder UseCheckAuthorizationToken(
            this IApplicationBuilder builder
        )
        {
            return builder.UseMiddleware<VerifyAuthorizationTokenMiddleware>();
        }
    }
}
