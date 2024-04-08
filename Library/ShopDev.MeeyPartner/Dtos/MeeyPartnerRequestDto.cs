using System.Text.Json.Serialization;
using CR.MeeyPartner.Constants;

namespace CR.MeeyPartner.Dtos
{
    public class MeeyPartnerSendOtpRequestDto
    {
        [JsonPropertyName("phone")]
        public required string Phone { get; set; }

        [JsonPropertyName("phoneCode")]
        public string? PhoneCode { get; set; } = MeeyPartnerPhoneCodes.Vietnamese;
    }

    public class MeeyPartnerResendOtpRequestDto : MeeyPartnerSendOtpRequestDto
    {
        [JsonPropertyName("requestId")]
        public required string RequestId { get; set; }
    }

    public class MeeyPartnerVerifyOtpRequestDto : MeeyPartnerResendOtpRequestDto
    {
        [JsonPropertyName("otp")]
        public required string Otp { get; set; }
    }
}
