namespace CR.MeeyPartner.Configs
{
    public class MeeyPartnerConfig
    {
        public const string ApiSendOtp = "partner/v1/sendOtp";
        public const string ApiResendOtp = "partner/v1/resendOtp";
        public const string ApiVerifyOtp = "partner/v1/verifyOtp";
        public const string ApiSendSms = "partner/v1/sendSms";
        public string BaseUrl { get; set; } = null!;
        public string ClientId { get; set; } = null!;
        public string ClientKey { get; set; } = null!;
    }
}
