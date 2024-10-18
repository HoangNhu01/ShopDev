using System.Text.Json.Serialization;

namespace CR.MeeyPartner.Dtos
{
    public class MeeyPartnerResendOtpResponseDto
    {
        [JsonPropertyName("error")]
        public MeeyPartnerResendOtpResponseErrorDto Error { get; set; } = new()!;

        [JsonPropertyName("data")]
        public MeeyPartnerResendOtpResponseDataDto? Data { get; set; }
    }

    public class MeeyPartnerResendOtpResponseErrorDto
    {
        [JsonPropertyName("status")]
        public bool? Status { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("code")]
        public object? Code { get; set; }
    }

    public class MeeyPartnerResendOtpResponseDataDto
    {
        [JsonPropertyName("requestId")]
        public string? RequestId { get; set; }
    }
}
