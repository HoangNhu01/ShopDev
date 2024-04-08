using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CR.MeeyPartner.Dtos
{
    public class MeeyPartnerVerifyOtpResponseDto
    {
        [JsonPropertyName("error")]
        public MeeyPartnerResendOtpResponseErrorDto Error { get; set; } = new()!;

        [JsonPropertyName("data")]
        public MeeyPartnerResendOtpResponseDataDto? Data { get; set; }
    }

    public class MeeyPartnerVerifyOtpResponseErrorDto
    {
        [JsonPropertyName("status")]
        public bool? Status { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("code")]
        public object? Code { get; set; }
    }

    public class MeeyPartnerVerifyOtpResponseDataDto
    {
        [JsonPropertyName("status")]
        public int? Status { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("countRemain")]
        public int? CountRemain { get; set; }

        [JsonPropertyName("remain")]
        public int? Remain { get; set; }
    }
}
