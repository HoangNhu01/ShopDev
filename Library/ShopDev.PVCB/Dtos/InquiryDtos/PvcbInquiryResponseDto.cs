using System.Text.Json.Serialization;

namespace CR.PVCB.Dtos.InquiryDtos
{
    /// <summary>
    /// Response truy vấn tài khoản ngân hàng
    /// </summary>
    public class PvcbInquiryResponseDto
    {
        [JsonPropertyName("Data")]
        public PvcbInquiryResponseData? Data { get; set; }

        [JsonPropertyName("Meta")]
        public PvcbInquiryResponseMeta? Meta { get; set; }
    }

    public class PvcbInquiryRepsonseCreditorAccount
    {
        [JsonPropertyName("SourceNumber")]
        public string? SourceNumber { get; set; }

        [JsonPropertyName("SourceType")]
        public string? SourceType { get; set; }

        [JsonPropertyName("SourceName")]
        public string? SourceName { get; set; }
    }

    public class PvcbInquiryResponseData
    {
        [JsonPropertyName("TransId")]
        public string? TransId { get; set; }

        [JsonPropertyName("DateTime")]
        public DateTime? DateTime { get; set; }

        [JsonPropertyName("CreditorAccount")]
        public PvcbInquiryRepsonseCreditorAccount? CreditorAccount { get; set; }
    }

    public class PvcbInquiryResponseMeta
    {
        [JsonPropertyName("RefNum")]
        public string? RefNum { get; set; }

        [JsonPropertyName("AuId")]
        public string? AuId { get; set; }
    }
}
