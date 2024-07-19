namespace ShopDev.ApplicationBase.Common
{
    /// <summary>
    /// Cấu hình identity
    /// </summary>
    public static class IdentityServerConfigs
    {
        /// <summary>
        /// Thời gian sống của access token
        /// </summary>
        public static readonly TimeSpan AccessTokenLifetime = TimeSpan.FromHours(2);
        /// <summary>
        /// Thời gian sống của refresh token
        /// </summary>
        public static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(3);
        /// <summary>
        /// Thời gian sống của user code
        /// </summary>
        public static readonly TimeSpan UserCodeLifetime = TimeSpan.FromSeconds(30);
        /// <summary>
        /// Ngưỡng xoá token để dọn dẹp db, các token trước thời điểm này sẽ bị xoá
        /// </summary>
        public static readonly DateTimeOffset ThresholdPruned = DateTimeOffset.Now.AddDays(-7);
    }
}
