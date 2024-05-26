using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using ShopDev.Constants.Users;

namespace ShopDev.Common
{
    public static class HttpContextExtensions
    {
        public static int GetCurrentUserType(this IHttpContextAccessor httpContextAccessor)
        {
            var claims = httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;
            var claim = claims?.FindFirst(UserClaimTypes.UserType);
            return claim == null
                ? throw new InvalidOperationException($"Claim {UserClaimTypes.UserType} not found.")
                : int.Parse(claim!.Value!);
        }

        public static int GetCurrentUserId(this IHttpContextAccessor httpContextAccessor)
        {
            var claims = httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;
            var claim =
                (claims?.FindFirst(UserClaimTypes.UserId))
                ?? throw new InvalidOperationException($"Claim {UserClaimTypes.UserId} not found.");
            int userId = int.Parse(claim.Value);
            return userId;
        }

        public static int GetCurrentCustomerId(this IHttpContextAccessor httpContextAccessor)
        {
            var claims = httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;
            var claim =
                (claims?.FindFirst(UserClaimTypes.CustomerId))
                ?? throw new InvalidOperationException(
                    $"Claim {UserClaimTypes.CustomerId} not found."
                );
            int customerId = int.Parse(claim.Value);
            return customerId;
        }

        public static string RandomNumber(int length = 6)
        {
            Random random = new();
            const string chars = "0123456789";

            return new string(
                Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray()
            );
        }

        public static string? GetCurrentRemoteIpAddress(
            this IHttpContextAccessor httpContextAccessor
        )
        {
            string? senderIpv4 = httpContextAccessor
                ?.HttpContext?.Connection?.RemoteIpAddress?.MapToIPv4()
                .ToString();
            if (
                httpContextAccessor?.HttpContext?.Request.Headers.TryGetValue(
                    "x-forwarded-for",
                    out var forwardedIps
                ) == true
            )
            {
                senderIpv4 = forwardedIps.FirstOrDefault();
            }
            return senderIpv4;
        }

        public static string? GetCurrentXForwardedFor(this IHttpContextAccessor httpContextAccessor)
        {
            string? forwardedIpsStr = null;
            if (
                httpContextAccessor?.HttpContext?.Request.Headers.TryGetValue(
                    "x-forwarded-for",
                    out var forwardedIps
                ) == true
            )
            {
                forwardedIpsStr = JsonSerializer.Serialize(forwardedIps.ToList());
            }
            return forwardedIpsStr;
        }

        public static TService GetService<TService>(this IHttpContextAccessor httpContextAccessor)
            where TService : class
        {
            var service =
                httpContextAccessor?.HttpContext?.RequestServices.GetService(typeof(TService))
                    as TService
                ?? throw new InvalidOperationException(
                    $"Can not resolve service type: {typeof(TService)}"
                );
            return service;
        }
    }
}
