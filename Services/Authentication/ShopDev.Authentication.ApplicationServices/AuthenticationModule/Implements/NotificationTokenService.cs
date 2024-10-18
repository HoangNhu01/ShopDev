using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShopDev.Authentication.ApplicationServices.AuthenticationModule.Abstract;
using ShopDev.Authentication.ApplicationServices.Common;
using ShopDev.Authentication.Domain.AuthToken;

namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Implements
{
    public class NotificationTokenService : AuthenticationServiceBase, INotificationTokenService
    {
        public NotificationTokenService(
            ILogger<NotificationTokenService> logger,
            IHttpContextAccessor httpContext
        )
            : base(logger, httpContext) { }

        public async Task AddNotificationToken(string? fcmToken, string? apnsToken)
        {
            var userId = _httpContext.GetCurrentUserId();
            _logger.LogInformation(
                $"{nameof(AddNotificationToken)}: userId = {userId}, fcmToken = {fcmToken}, apnsToken = {apnsToken}"
            );
            var transaction = await _dbContext.Database.BeginTransactionAsync();
            await _dbContext.NotificationTokens.Where(x => x.UserId == userId).ExecuteDeleteAsync();
            await _dbContext.AddAsync<NotificationToken>(
                new()
                {
                    FcmToken = fcmToken,
                    ApnsToken = apnsToken,
                    UserId = userId,
                }
            );
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
    }
}
