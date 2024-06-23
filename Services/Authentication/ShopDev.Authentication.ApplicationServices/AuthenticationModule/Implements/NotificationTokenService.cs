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

        public void AddNotificationToken(int userId, string? fcmToken, string? apnsToken)
        {
            _logger.LogInformation(
                $"{nameof(AddNotificationToken)}: userId = {userId}, fcmToken = {fcmToken}, apnsToken = {apnsToken}"
            );
            var transaction = _dbContext.Database.BeginTransaction();
            _dbContext.NotificationTokens.Where(x => x.UserId == userId).ExecuteDelete();
            _dbContext.Add<NotificationToken>(
                new()
                {
                    FcmToken = fcmToken,
                    ApnsToken = apnsToken,
                    UserId = userId,
                }
            );
            _dbContext.SaveChanges();
            transaction.Commit();
        }
    }
}
