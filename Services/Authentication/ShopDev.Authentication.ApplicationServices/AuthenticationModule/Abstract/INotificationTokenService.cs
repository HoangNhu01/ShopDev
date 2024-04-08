namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Abstract
{
    public interface INotificationTokenService
    {
        /// <summary>
        /// Thêm mới authtoken khi đăng nhập
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fcmToken"></param>
        /// <param name="apnsToken"></param>
        void AddNotificationToken(int userId, string? fcmToken, string? apnsToken);
    }
}
