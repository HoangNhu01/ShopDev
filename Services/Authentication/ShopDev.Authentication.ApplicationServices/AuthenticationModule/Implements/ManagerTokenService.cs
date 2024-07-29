using AutoMapper;
using Hangfire;
using MB.Authentication.ApplicationServices.AuthenticationModule.Abstract;
using ShopDev.Utils.Cache;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;
using ShopDev.ApplicationBase.Common;
using ShopDev.ApplicationBase.Localization;
using ShopDev.Authentication.ApplicationServices.Common;
using ShopDev.Authentication.Infrastructure.Persistence;
using ShopDev.Constants.Common.Cache;
using ShopDev.InfrastructureBase.Hangfire.Attributes;
using StackExchange.Redis;

namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Implements
{
    public class ManagerTokenService : AuthenticationServiceBase, IManagerTokenService
    {
        private readonly IOpenIddictTokenManager _tokenManager;
        private readonly ConnectionMultiplexer _connectionMultiplexerRedis;
        private readonly IDistributedCache _distributedCache;
        private readonly IOpenIddictAuthorizationManager _openIddictAuthorizationManager;

        public ManagerTokenService(
            ILogger<ManagerTokenService> logger,
            IMapErrorCode mapErrorCode,
            IHttpContextAccessor httpContext,
            AuthenticationDbContext dbContext,
            LocalizationBase localizationBase,
            IMapper mapper,
            IOpenIddictTokenManager tokenManager,
            ConnectionMultiplexer connectionMultiplexerRedis,
            IDistributedCache distributedCache,
            IOpenIddictAuthorizationManager openIddictAuthorizationManager
        )
            : base(logger, mapErrorCode, httpContext, dbContext, localizationBase, mapper)
        {
            _tokenManager = tokenManager;
            _connectionMultiplexerRedis = connectionMultiplexerRedis;
            _distributedCache = distributedCache;
            _openIddictAuthorizationManager = openIddictAuthorizationManager;
        }

        public async Task RevokeOtherToken()
        {
            var subject = _httpContext.GetCurrentSubject();
            var authorizationId = _httpContext.GetCurrentAuthorizationId();

            var tokenByAuthorizationId = _tokenManager.FindByAuthorizationIdAsync(authorizationId);
            List<string?> currentTokenIds = new();
            await foreach (var token in tokenByAuthorizationId)
            {
                if (token is OpenIddictEntityFrameworkCoreToken openIdDictToken)
                {
                    currentTokenIds.Add(openIdDictToken.Id);
                }
            }

            var tokens = _tokenManager.FindBySubjectAsync(subject);
            await foreach (var token in tokens)
            {
                if (
                    token is OpenIddictEntityFrameworkCoreToken openIdDictToken
                    && openIdDictToken.Status == OpenIddictConstants.Statuses.Valid
                    && !currentTokenIds.Contains(openIdDictToken.Id)
                )
                {
                    await _tokenManager.TryRevokeAsync(token);

                    var tokenData = (OpenIddictEntityFrameworkCoreToken)token;
                    // Xóa cache token theo tokenId
                    await _distributedCache!.RemoveByPatternAsync(
                        _connectionMultiplexerRedis,
                        PrefixKeyCache.PatternString(
                            PrefixKeyCache.AuthToken,
                            authorizationId + tokenData.Id
                        )
                    );
                }
            }
        }

        public void RevokeAllTokenBySubject()
        {
            var subject = _httpContext.GetCurrentSubject();
            var tokenRevoke = _dbContext
                .OpenIddictAuthorizations.Where(x => x.Subject == subject)
                .SelectMany(x => x.Tokens)
                .ExecuteUpdate(p =>
                    p.SetProperty(x => x.Status, OpenIddictConstants.Statuses.Revoked)
                );
        }

        public async Task RevokeAllToken()
        {
            var authorizationId = _httpContext.GetCurrentAuthorizationId();
            var tokens = _tokenManager.FindByAuthorizationIdAsync(authorizationId);
            await foreach (var token in tokens)
            {
                if (
                    token is OpenIddictEntityFrameworkCoreToken openIdDictToken
                    && openIdDictToken.Status == OpenIddictConstants.Statuses.Valid
                )
                {
                    await _tokenManager.TryRevokeAsync(token);
                    if (
                        openIdDictToken.Type == OpenIddictConstants.Parameters.AccessToken
                        && openIdDictToken.ExpirationDate != null
                        && openIdDictToken.CreationDate != null
                    )
                    {
                        string key = PrefixKeyCache.AuthTokenRevoke + openIdDictToken.Id;
                        TimeSpan timeDifference = openIdDictToken.ExpirationDate.Value.Subtract(
                            openIdDictToken.CreationDate.Value
                        );
                        var epiredTimes = timeDifference.TotalSeconds;
                        await _distributedCache!.SetAsync(
                            key,
                            OpenIddictConstants.Statuses.Revoked,
                            (int)epiredTimes
                        );
                    }
                }
            }

            // Xóa cache token theo authorizationId
            await _distributedCache.RemoveByPatternAsync(
                _connectionMultiplexerRedis,
                PrefixKeyCache.PatternString(PrefixKeyCache.AuthToken, authorizationId)
            );
        }

        [AutomaticRetry(Attempts = 3)]
        [HangfireLogEverything]
        public async Task PruneTokenByThreshold()
        {
            await _tokenManager.PruneAsync(IdentityServerConfigs.ThresholdPruned);
            await _openIddictAuthorizationManager.PruneAsync(IdentityServerConfigs.ThresholdPruned);
        }
    }
}
