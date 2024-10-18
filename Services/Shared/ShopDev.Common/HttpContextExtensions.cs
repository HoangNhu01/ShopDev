using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using ShopDev.Constants.Users;

namespace ShopDev.Common
{
    public static class HttpContextExtensions
    {
        public static string GetLocalIPAddress()
        {
            string hostName = Dns.GetHostName();
            IPAddress[] addresses = Dns.GetHostAddresses(hostName);

            foreach (IPAddress address in addresses)
            {
                // Filter for IPv4 addresses
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return address.ToString();
                }
            }

            return string.Empty;
        }

        public static int GetCurrentUserType(this IHttpContextAccessor httpContextAccessor)
        {
            var claims = httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;
            var claim = claims?.FindFirst(Constants.Users.ClaimTypes.UserType);
            return claim == null
                ? throw new InvalidOperationException(
                    $"Claim {Constants.Users.ClaimTypes.UserType} not found."
                )
                : int.Parse(claim!.Value!);
        }

        private static Claim FindClaim(
            this IHttpContextAccessor httpContextAccessor,
            string claimType
        )
        {
            var claims = httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;
            var claim =
                claims?.FindFirst(claimType)
                ?? throw new InvalidOperationException($"Claim \"{claimType}\" not found.");
            return claim;
        }

        public static int GetCurrentUserId(this IHttpContextAccessor httpContextAccessor)
        {
            var claims = httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;
            var claim =
                (claims?.FindFirst(Constants.Users.ClaimTypes.UserId))
                ?? throw new InvalidOperationException(
                    $"Claim {Constants.Users.ClaimTypes.UserId} not found."
                );
            int userId = int.Parse(claim.Value);
            return userId;
        }

        public static string GetCurrentAuthorizationId(
            this IHttpContextAccessor httpContextAccessor
        )
        {
            var claim = httpContextAccessor.FindClaim(Constants.Users.ClaimTypes.AuthorizationId);
            return claim.Value;
        }

        public static string GetCurrentSubject(this IHttpContextAccessor httpContextAccessor)
        {
            var claim = httpContextAccessor.FindClaim(
                System.Security.Claims.ClaimTypes.NameIdentifier
            );
            return claim.Value;
        }

        public static int GetCurrentCustomerId(this IHttpContextAccessor httpContextAccessor)
        {
            var claims = httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;
            var claim =
                (claims?.FindFirst(Constants.Users.ClaimTypes.CustomerId))
                ?? throw new InvalidOperationException(
                    $"Claim {Constants.Users.ClaimTypes.CustomerId} not found."
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
