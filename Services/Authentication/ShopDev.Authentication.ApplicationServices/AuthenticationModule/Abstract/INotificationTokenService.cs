namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Abstract
{
    public interface INotificationTokenService
    {
        /// <summary>
        /// Thêm mới authtoken cho user hiện tại
        /// </summary>
        /// <param name="fcmToken"></param>
        /// <param name="apnsToken"></param>
        Task AddNotificationToken(string? fcmToken, string? apnsToken);
    }
}
