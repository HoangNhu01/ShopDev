using ShopDev.Authentication.ApplicationServices.AuthenticationModule.Abstract;
using ShopDev.Authentication.ApplicationServices.Common;
using ShopDev.Authentication.Domain.AuthToken;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
            var authTokens = _dbContext.NotificationTokens;
            var auths = authTokens.Where(x => x.UserId == userId).ExecuteDelete();
            authTokens.Add(
                new NotificationToken
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
