using System.Security.Claims;
using System.Web;
using MB.Authentication.ApplicationServices.AuthenticationModule.Abstract;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using ShopDev.ApplicationBase.Localization;
using ShopDev.Authentication.API.Extensions;
using ShopDev.Authentication.API.Models;
using ShopDev.Authentication.ApplicationServices.AuthenticationModule.Abstract;
using ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.UserDto;
using ShopDev.Constants.Authorization;
using ShopDev.Constants.ErrorCodes;
using ShopDev.Constants.Users;
using ShopDev.IdentityServerBase.Constants;
using ShopDev.IdentityServerBase.Controllers;
using ShopDev.IdentityServerBase.Dto;
using ShopDev.InfrastructureBase.Exceptions;
using StackExchange.Profiling.Internal;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ShopDev.Authentication.API.Controllers
{
    [ApiController]
    public class AuthorizationController : AuthorizationControllerBase
    {
        private readonly IUserService _userServices;
        private readonly IMapErrorCode _mapErrorCode;
        private readonly INotificationTokenService _authTokenService;
        private readonly LocalizationBase _localization;
        private readonly IOpenIddictScopeManager _scopeManager;
        private readonly IOpenIddictTokenManager _tokenManager;
        private readonly IManagerTokenService _managerTokenService;
        private readonly ILogger<AuthorizationController> _logger;

        public AuthorizationController(
            INotificationTokenService authTokenService,
            IOpenIddictApplicationManager applicationManager,
            IOpenIddictTokenManager tokenManager,
            IUserService userServices,
            ILogger<AuthorizationController> logger,
            IMapErrorCode mapErrorCode,
            IOpenIddictScopeManager scopeManager,
            LocalizationBase localization
        )
            : base(applicationManager)
        {
            _logger = logger;
            _authTokenService = authTokenService;
            _tokenManager = tokenManager;
            _userServices = userServices;
            _mapErrorCode = mapErrorCode;
            _localization = localization;
            _scopeManager = scopeManager;
        }

        /// <summary>
        /// Xác thực đăng nhập
        /// </summary>
        [HttpGet("~/connect/authorize")]
        [HttpPost("~/connect/authorize")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Authorize()
        {
            var request = HttpContext.GetOpenIddictServerRequest()!;

            var parameters = ParseOAuthParameters(
                HttpContext,
                new List<string> { Parameters.Prompt }
            );

            var result = await HttpContext.AuthenticateAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            // KIểm tra đã đăng nhập chưa
            if (!IsAuthenticated(result, request))
            {
                // Điều hướng đến trang đăng nhập kèm ReturnUrl trở lại
                return Challenge(
                    properties: new AuthenticationProperties
                    {
                        RedirectUri = BuildRedirectUrl(HttpContext.Request, parameters)
                    },
                    CookieAuthenticationDefaults.AuthenticationScheme
                );
            }

            var userId = result.Principal!.GetClaim(Constants.Users.ClaimTypes.UserId);

            var identity = new ClaimsIdentity(
                TokenValidationParameters.DefaultAuthenticationType,
                Claims.Name,
                Claims.Role
            );

            var application =
                await _applicationManager.FindByClientIdAsync(request.ClientId!)
                ?? throw new InvalidOperationException("Không tìm thấy CientId");

            var consentType = await _applicationManager.GetConsentTypeAsync(application);
            switch (consentType)
            {
                case ConsentTypes.External:
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(
                            new Dictionary<string, string?>
                            {
                                [OpenIddictServerAspNetCoreConstants.Properties.Error] =
                                    Errors.ConsentRequired,
                                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                                    "The logged in user is not allowed to access this client application."
                            }
                        )
                    );
                case ConsentTypes.Implicit:
                case ConsentTypes.Explicit
                    when result.Principal!.GetClaim(Prompts.Consent) == ConsentValue.Grant:

                    identity
                        .SetClaim(Claims.Subject, userId)
                        .SetClaim(Constants.Users.ClaimTypes.UserId, userId);
                    identity.SetScopes(request.GetScopes());
                    identity.SetResources(
                        await _scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync()
                    );
                    identity.SetDestinations(GetDestinations);

                    return SignIn(
                        new ClaimsPrincipal(identity),
                        OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                    );

                case ConsentTypes.Explicit
                    when result.Principal!.GetClaim(Prompts.Consent) != ConsentValue.Grant:
                    var returnUrl = HttpUtility.UrlEncode(
                        BuildRedirectUrl(HttpContext.Request, parameters)
                    );
                    var consentRedirectUrl = $"/authenticate/consent?returnUrl={returnUrl}";
                    return Redirect(consentRedirectUrl);
                case ConsentTypes.Explicit when request.HasPrompt(Prompts.None):
                case ConsentTypes.Systematic when request.HasPrompt(Prompts.None):
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(
                            new Dictionary<string, string?>
                            {
                                [OpenIddictServerAspNetCoreConstants.Properties.Error] =
                                    Errors.ConsentRequired,
                                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                                    "Interactive user consent is required."
                            }
                        )
                    );
                default:
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(
                            new Dictionary<string, string?>
                            {
                                [OpenIddictServerAspNetCoreConstants.Properties.Error] =
                                    Errors.ConsentRequired,
                                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                                    $"ConsentType: \"{consentType}\" is not valid."
                            }
                        )
                    );
            }
        }

        /// <summary>
        /// Màn hình đăng nhập
        /// </summary>
        [HttpGet("authenticate/login")]
        public IActionResult Authenticate([FromQuery] string? returnUrl)
        {
            if (returnUrl == null)
            {
                return View();
            }
            AuthenticateModel model = new() { ReturnUrl = returnUrl, };
            return View(model);
        }

        /// <summary>
        /// Gửi thông tin đăng nhập
        /// </summary>
        [HttpPost("authenticate/login")]
        //[EnableRateLimiting(LimitRatePolicyName.LimitLogin)]
        public async Task<IActionResult> AuthenticateAsync([FromForm] AuthenticateModel input)
        {
            try
            {
                if (string.IsNullOrEmpty(input.Username) || string.IsNullOrEmpty(input.Password))
                {
                    ViewBag.UsernameError = string.IsNullOrEmpty(input.Username)
                        ? _localization.Localize("error_validation_AuthorizationUsername")
                        : null;
                    ViewBag.PasswordError = string.IsNullOrEmpty(input.Password)
                        ? _localization.Localize("error_validation_AuthorizationPassword")
                        : null;
                    return View("Authenticate", input);
                }
                var user = _userServices.ValidateAdmin(input.Username!, input.Password!);

                var identity = new ClaimsIdentity(
                    CookieAuthenticationDefaults.AuthenticationScheme
                );
                SetClaims(identity, user);
                var principal = new ClaimsPrincipal(new List<ClaimsIdentity> { new(identity) });

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal
                );

                if (!string.IsNullOrEmpty(input.ReturnUrl))
                {
                    return Redirect(input.ReturnUrl);
                }
            }
            catch (UserFriendlyException ex)
            {
                var mapErrorCode = HttpContext.RequestServices.GetRequiredService<IMapErrorCode>();
                var localization =
                    HttpContext.RequestServices.GetRequiredService<LocalizationBase>();
                ViewBag.ErrorLogin = localization.Localize(
                    mapErrorCode.GetErrorMessageKey(ex.ErrorCode),
                    ex.ListParam
                );
                mapErrorCode.GetErrorMessage(ex.ErrorCode);
            }

            return View("Authenticate", input);
        }

        /// <summary>
        /// Xác nhận đăng nhập
        /// </summary>
        [HttpGet("authenticate/consent")]
        public IActionResult Consent([FromQuery] string? returnUrl)
        {
            if (returnUrl == null)
            {
                return View("authenticate");
            }
            ConsentModel model = new ConsentModel { ReturnUrl = returnUrl, };
            return View("consent", model);
        }

        /// <summary>
        /// Gửi thông tin xác thực
        /// </summary>
        [HttpPost("authenticate/consent")]
        public async Task<IActionResult> ConsentAsync([FromForm] ConsentModel dto)
        {
            var result = await HttpContext.AuthenticateAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity
                .SetClaim(Claims.Subject, result.Principal!.GetClaim(Claims.Subject))
                .SetClaim(
                    Constants.Users.ClaimTypes.UserId,
                    result.Principal!.GetClaim(Constants.Users.ClaimTypes.UserId)
                )
                .SetClaim(Prompts.Consent, dto.Grant);

            var principal = new ClaimsPrincipal(new List<ClaimsIdentity> { new(identity) });

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );
            return Redirect(dto.ReturnUrl ?? string.Empty);
        }

        [
            HttpPost("~/connect/token"),
            IgnoreAntiforgeryToken,
            Produces("application/json"),
            Consumes("application/x-www-form-urlencoded")
        ]
        public async Task<IActionResult> Exchange([FromForm] ConnectTokenDto _)
        {
            // Create a new ClaimsIdentity containing the claims that
            // will be used to create an id_token, a token or a code.
            var identity = new ClaimsIdentity(
                TokenValidationParameters.DefaultAuthenticationType,
                Claims.Name,
                Claims.Role
            );
            //string localizationName = Request.HttpContext.Items[LocalizationQuery.QueryName]!.ToString()!;

            var request = HttpContext.GetOpenIddictServerRequest()!;
            try
            {
                if (request.IsAuthorizationCodeGrantType())
                {
                    var result = await HttpContext.AuthenticateAsync(
                        OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                    );
                    int userId = int.Parse(
                        result.Principal!.GetClaim(Constants.Users.ClaimTypes.UserId)!
                    );

                    var user =
                        _userServices.FindById(userId)
                        ?? throw new UserFriendlyException(ErrorCode.UserNotFound);

                    SetClaims(identity, user);
                    // Set the list of scopes granted to the client application.
                    identity.SetScopes(
                        new[]
                        {
                            Scopes.OpenId,
                            Scopes.Email,
                            Scopes.Profile,
                            Scopes.Roles,
                            Scopes.OfflineAccess
                        }.Intersect(request.GetScopes())
                    );

                    identity.SetDestinations(GetDestinations);

                    //var ticket = new AuthenticationTicket(
                    //    new ClaimsPrincipal(identity),
                    //    new AuthenticationProperties(),
                    //    OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                    return SignIn(
                        new ClaimsPrincipal(identity),
                        OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                    );
                }
                else if (request.IsAppGrantType())
                {
                    var user = _userServices.ValidateAppUser(request.Username!, request.Password!);
                    _userServices.Login(user.Id);
                    await _authTokenService.AddNotificationToken(_.FcmToken, _.ApnsToken);
                    SetClaims(identity, user);
                    // Set the list of scopes granted to the client application.
                    identity.SetScopes(
                        new[]
                        {
                            Scopes.OpenId,
                            Scopes.Email,
                            Scopes.Profile,
                            Scopes.Roles,
                            Scopes.OfflineAccess
                        }.Intersect(request.GetScopes())
                    );
                    identity.SetDestinations(GetDestinations);
                    var authenticationProperties = new AuthenticationProperties();
                    authenticationProperties.SetParameter(AuthParameters.IsTempPin, user.IsTempPin);
                    authenticationProperties.SetParameter(
                        AuthParameters.IsTempPassword,
                        user.IsPasswordTemp
                    );
                    authenticationProperties.SetParameter(
                        AuthParameters.IsHasPin,
                        user.PinCode.HasValue()
                    );
                    return SignIn(
                        new ClaimsPrincipal(identity),
                        authenticationProperties,
                        OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                    );
                }
                else if (request.IsClientCredentialsGrantType())
                {
                    // Note: the client credentials are automatically validated by OpenIddict:
                    // if client_id or client_secret are invalid, this action won't be invoked.

                    var application =
                        await _applicationManager.FindByClientIdAsync(request.ClientId!)
                        ?? throw new InvalidOperationException("The application cannot be found.");

                    // Use the client_id as the subject identifier.
                    identity.SetClaim(
                        Claims.Subject,
                        await _applicationManager.GetClientIdAsync(application)
                    );
                    identity.SetClaim(
                        Claims.Name,
                        await _applicationManager.GetDisplayNameAsync(application)
                    );

                    identity.SetDestinations(static claim =>
                        claim.Type switch
                        {
                            // Allow the "name" claim to be stored in both the access and identity tokens
                            // when the "profile" scope was granted (by calling principal.SetScopes(...)).
                            Claims.Name when claim.Subject?.HasScope(Scopes.Profile) == true
                                => [Destinations.AccessToken, Destinations.IdentityToken],

                            // Otherwise, only store the claim in the access tokens.
                            _ => new[] { Destinations.AccessToken }
                        }
                    );
                    return SignIn(
                        new ClaimsPrincipal(identity),
                        OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                    );
                }
                else if (request.IsPasswordGrantType())
                {
                    var user = _userServices.ValidateAdmin(request.Username!, request.Password!);
                    _userServices.Login(user.Id);
                    SetClaims(identity, user);

                    // Set the list of scopes granted to the client application.
                    identity.SetScopes(
                        new[]
                        {
                            Scopes.OpenId,
                            Scopes.Email,
                            Scopes.Profile,
                            Scopes.Roles,
                            Scopes.OfflineAccess
                        }.Intersect(request.GetScopes())
                    );

                    identity.SetDestinations(GetDestinations);

                    //var ticket = new AuthenticationTicket(
                    //    new ClaimsPrincipal(identity),
                    //    new AuthenticationProperties(),
                    //    OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

                    return SignIn(
                        new ClaimsPrincipal(identity),
                        OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                    );
                }
                else if (request.IsRefreshTokenGrantType())
                {
                    // Retrieve the claims principal stored in the refresh token.
                    var result = await HttpContext.AuthenticateAsync(
                        OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                    );
                    // Retrieve the user profile corresponding to the refresh token.
                    var user =
                        _userServices.FindById(
                            int.Parse(
                                result.Principal!.GetClaim(Constants.Users.ClaimTypes.UserId)!
                            )
                        ) ?? throw new UserFriendlyException(ErrorCode.UserNotFound);

                    // Ensure the user is still allowed to sign in.
                    if (user.Status != UserStatus.ACTIVE)
                    {
                        throw new UserFriendlyException(ErrorCode.UserIsDeactive);
                    }

                    // Override the user claims present in the principal in case they changed since the refresh token was issued.
                    // Add the claims that will be persisted in the tokens.
                    SetClaims(identity, user);

                    identity.SetDestinations(GetDestinations);

                    return SignIn(
                        new ClaimsPrincipal(identity),
                        OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                    );
                }
            }
            catch (UserFriendlyException ex)
            {
                var properties = new AuthenticationProperties(
                    new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] =
                            Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            _localization.Localize(
                                _mapErrorCode.GetErrorMessageKey(ex.ErrorCode),
                                ex.ListParam
                            )
                    }
                );
                return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }
            catch (Exception ex)
            {
                var properties = new AuthenticationProperties(
                    new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ServerError,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            ex.Message
                    }
                );
                return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }
            return BadRequest(
                new OpenIddictResponse
                {
                    Error = Errors.UnsupportedGrantType,
                    ErrorDescription = "The specified grant type is not supported."
                }
            );
        }

        /// <summary>
        /// Xóa Cookies đăng nhập
        /// </summary>
        [HttpGet("authenticate/logout")]
        public async Task<IActionResult> AuthenticateLogout(string returnUrl)
        {
            // Xóa Cookies
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect(returnUrl);
        }

        /// <summary>
        /// Đăng xuất và revoke token
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost("~/connect/logout"), IgnoreAntiforgeryToken, Produces("application/json")]
        public async Task<IActionResult> Logout([FromForm] bool? revokeAll = false)
        {
            if (revokeAll == false)
            {
                await _managerTokenService.RevokeAllToken();
            }
            else
            {
                // Remove tất cả token theo subject
                _managerTokenService.RevokeAllTokenBySubject();
            }
            return SignOut(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        private void SetClaims(ClaimsIdentity identity, UserDto user)
        {
            // Add the claims that will be persisted in the tokens.
            identity
                .SetClaim(Claims.Username, user.Username)
                .SetClaim(Claims.Subject, user.Id.ToString())
                .SetClaim(Claims.Issuer, $"{Request.Scheme}://{Request.Host.Value}")
                .SetClaim(Claims.Name, user.FullName)
                .SetClaim(Constants.Users.ClaimTypes.UserType, user.UserType)
                .SetClaim(Constants.Users.ClaimTypes.UserId, user.Id);
        }

        private static IEnumerable<string> GetDestinations(Claim claim)
        {
            // Note: by default, claims are NOT automatically included in the access and identity tokens.
            // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
            // whether they should be included in access tokens, in identity tokens or in both.

            switch (claim.Type)
            {
                case Claims.Name:
                    yield return Destinations.AccessToken;

                    if (claim.Subject?.HasScope(Scopes.Profile) == true)
                        yield return Destinations.IdentityToken;

                    yield break;

                case Claims.Email:
                    yield return Destinations.AccessToken;

                    if (claim.Subject?.HasScope(Scopes.Email) == true)
                        yield return Destinations.IdentityToken;

                    yield break;

                case Claims.Role:
                    yield return Destinations.AccessToken;

                    if (claim.Subject?.HasScope(Scopes.Roles) == true)
                        yield return Destinations.IdentityToken;

                    yield break;

                // Never include the security stamp in the access and identity tokens, as it's a secret value.
                case "AspNet.Identity.SecurityStamp":
                    yield break;

                default:
                    yield return Destinations.AccessToken;
                    yield break;
            }
        }

        /// <summary>
        /// Kiểm ra xác thực
        /// </summary>
        private bool IsAuthenticated(
            AuthenticateResult authenticateResult,
            OpenIddictRequest request
        )
        {
            if (!authenticateResult.Succeeded)
            {
                return false;
            }

            // Kiểm tra hết hạn code
            if (request.MaxAge.HasValue && authenticateResult.Properties != null)
            {
                var maxAgeSeconds = TimeSpan.FromSeconds(request.MaxAge.Value);

                var expired =
                    !authenticateResult.Properties.IssuedUtc.HasValue
                    || DateTimeOffset.UtcNow - authenticateResult.Properties.IssuedUtc
                        > maxAgeSeconds;
                if (expired)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Đường dẫn điều hướng
        /// </summary>
        private string BuildRedirectUrl(
            HttpRequest request,
            IDictionary<string, StringValues> oAuthParameters
        )
        {
            var url = request.PathBase + request.Path + QueryString.Create(oAuthParameters);
            return url;
        }

        /// <summary>
        /// Lấy các giá trị của các tham số truyền vào
        /// </summary>
        private IDictionary<string, StringValues> ParseOAuthParameters(
            HttpContext httpContext,
            List<string>? excluding = null
        )
        {
            excluding ??= new List<string>();

            var parameters = httpContext.Request.HasFormContentType
                ? httpContext
                    .Request.Form.Where(v => !excluding.Contains(v.Key))
                    .ToDictionary(v => v.Key, v => v.Value)
                : httpContext
                    .Request.Query.Where(v => !excluding.Contains(v.Key))
                    .ToDictionary(v => v.Key, v => v.Value);
            return parameters;
        }
    }
}
