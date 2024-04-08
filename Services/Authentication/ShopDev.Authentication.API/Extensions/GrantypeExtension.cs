using ShopDev.IdentityServerBase.Constants;
using OpenIddict.Abstractions;

namespace ShopDev.Authentication.API.Extensions
{
    public static class GrantypeExtension
    {
        /// <summary>
        /// Determines whether the "grant_type" parameter corresponds to the password grant.
        /// See http://tools.ietf.org/html/rfc6749#section-4.3.2 for more information.
        /// </summary>
        /// <param name="request">The <see cref="OpenIddictRequest"/> instance.</param>
        /// <returns><see langword="true"/> if the request is a password grant request, <see langword="false"/> otherwise.</returns>
        public static bool IsAppGrantType(this OpenIddictRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return string.Equals(request.GrantType, GrantTypes.App, StringComparison.Ordinal);
        }
    }
}
