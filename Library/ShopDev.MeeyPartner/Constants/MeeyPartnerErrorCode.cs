using CR.EntitiesBase.Base;

namespace CR.MeeyPartner.Constants
{
    public class MeeyPartnerErrorCode : IErrorCode
    {
        public const int SendOtpNotSuccess = 15001;
        public const int ResendOtpNotSuccess = 15002;
        public const int VerifyOtpNotSuccess = 15003;
        public const int VerifyOtpTurnEnd = 15004;
        public const int VerifyOtpExpired = 15005;
        public const int SendOtpMutipleRequest = 15006;
    }
}
