namespace MB.Authentication.ApplicationServices.AuthenticationModule.Abstract
{
    public interface IManagerTokenService
    {
        /// <summary>
        /// Xoá token trước ngưỡng thời gian
        /// </summary>
        /// <returns></returns>
        Task PruneTokenByThreshold();

        /// <summary>
        /// Thu hồi tất cả token
        /// </summary>
        /// <returns></returns>
        Task RevokeAllToken();
        void RevokeAllTokenBySubject();

        /// <summary>
        /// Thu hồi tất cả token trừ token đang sử dụng hiện tại
        /// </summary>
        /// <returns></returns>
        Task RevokeOtherToken();
    }
}
