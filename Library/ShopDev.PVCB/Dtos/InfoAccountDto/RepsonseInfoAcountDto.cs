using System.Text.Json.Serialization;

namespace CR.PVCB.Dtos.InfoAccountDto
{
    public class PvcbInfoAcountCreditorAccount
    {
        [JsonPropertyName("SourceNumber")]
        public string? SourceNumber { get; set; }

        [JsonPropertyName("SourceType")]
        public string? SourceType { get; set; }

        [JsonPropertyName("SourceName")]
        public string? SourceName { get; set; }

        [JsonPropertyName("Amount")]
        public int Amount { get; set; }
    }

    public class PvcbInfoAcountData
    {
        [JsonPropertyName("TransId")]
        public string? TransId { get; set; }

        [JsonPropertyName("DateTime")]
        public DateTime DateTime { get; set; }

        [JsonPropertyName("CreditorAccount")]
        public PvcbInfoAcountCreditorAccount? CreditorAccount { get; set; }
    }

    /// <summary>
    /// Response trả về khi gọi api truy vấn số dư
    /// </summary>
    public class RepsonseInfoAcountDto
    {
        [JsonPropertyName("Data")]
        public PvcbInfoAcountData? Data { get; set; }
    }
}
