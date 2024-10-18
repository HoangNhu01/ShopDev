using System.Text.Json.Serialization;

namespace CR.MeeyPartner.Dtos
{
    public class MeeyPartnerSendOtpResponseDto
    {
        [JsonPropertyName("error")]
        public MeeyPartnerSendOtpResponseErrorDto Error { get; set; } = new()!;

        [JsonPropertyName("data")]
        public MeeyPartnerSendOtpResponseDataDto? Data { get; set; }
    }

    public class MeeyPartnerSendOtpResponseErrorDto
    {
        [JsonPropertyName("status")]
        public bool? Status { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("code")]
        public object? Code { get; set; }
    }

    public class MeeyPartnerSendOtpResponseDataDto
    {
        [JsonPropertyName("status")]
        public int? Status { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("data")]
        public string? Data { get; set; }

        [JsonPropertyName("meta")]
        public string? Meta { get; set; }
    }
}
