namespace CR.MeeyPartner
{
    public interface IMeeyPartnerService
    {
        /// <summary>
        /// Gửi mã Otp
        /// </summary>
        /// <param name="phone">Số điện thoại</param>
        /// <returns>Id yêu cầu Otp</returns>
        Task<string> SendOtp(string phone);

        /// <summary>
        /// Gửi lại mã Otp
        /// </summary>
        /// <param name="phone">Số điện thoại</param>
        /// <param name="requestId">Id yêu cầu Otp trước đó</param>
        /// <returns>Id yêu cầu Otp</returns>
        Task<string> ResendOtp(string phone, string requestId);

        /// <summary>
        /// Xác thực mã Otp
        /// </summary>
        /// <param name="phone">Số điện thoại</param>
        /// <param name="requestId">Id yêu cầu Otp trước đó</param>
        /// <param name="otp">Mã Otp</param>
        /// <returns></returns>
        Task VerifyOtp(string phone, string requestId, string otp);
    }
}
