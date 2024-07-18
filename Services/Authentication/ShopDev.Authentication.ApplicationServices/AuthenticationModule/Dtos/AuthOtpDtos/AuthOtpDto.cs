namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.AuthOtpDto
{
    public class AuthOtpDto
    {
        public int Id { get; set; }

        /// <summary>
        /// Otp code 6 kí tự số
        /// </summary>
        public required string OtpCode { get; set; }

        /// <summary>
        /// Thời gian hết hạn
        /// </summary>
        public DateTime ExpireTime { get; set; }

        /// <summary>
        /// Check đã được dùng
        /// </summary>
        public bool IsUsed { get; set; }

        /// <summary>
        /// Opt cho người nào
        /// </summary>
        public int UserId { get; set; }
    }
}
