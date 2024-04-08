namespace ShopDev.Constants.Users
{
    public static class UserStatus
    {
        /// <summary>
        /// Hoạt động
        /// </summary>
        public const int ACTIVE = 1;

        /// <summary>
        /// Đang khóa
        /// </summary>
        public const int DEACTIVE = 2;

        /// <summary>
        /// Xóa tài khoản (Xóa trên App)
        /// </summary>
        public const int LOCK = 3;

        /// <summary>
        /// Tạm: Đăng ký tài khoản chưa OTP
        /// </summary>
        public const int TEMP = 4;

        /// <summary>
        /// Tạm OTP : Đăng ký tài khoản đã xác thực OTP
        /// </summary>
        public const int TEMP_OTP = 5;
    }
}
