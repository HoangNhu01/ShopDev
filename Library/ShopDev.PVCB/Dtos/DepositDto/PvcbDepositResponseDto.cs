using System.Text.Json.Serialization;

namespace CR.PVCB.Dtos.DepositDto
{
    /// <summary>
    /// Request pvcb response
    /// </summary>
    public class PvcbDepositResponseDto
    {
        [JsonPropertyName("Data")]
        public PvcbDepositResponseData? Data { get; set; }

        [JsonPropertyName("Meta")]
        public PvcbDepositResponseMeta? Meta { get; set; }
    }

    public class PvcbDepositResponseData
    {
        [JsonPropertyName("InstructedAmount")]
        public PvcbDepositResponseInstructedAmount? InstructedAmount { get; set; }

        [JsonPropertyName("TransId")]
        public string? TransId { get; set; }

        [JsonPropertyName("DateTime")]
        public DateTime DateTime { get; set; }

        [JsonPropertyName("DebitorAccount")]
        public object? DebitorAccount { get; set; }

        [JsonPropertyName("CreditorAccount")]
        public PvcbDepositResponseCreditorAccount? CreditorAccount { get; set; }
    }

    public class PvcbDepositResponseInstructedAmount
    {
        [JsonPropertyName("Amount")]
        public int Amount { get; set; }

        [JsonPropertyName("Currency")]
        public string? Currency { get; set; }
    }

    public class PvcbDepositResponseMeta
    {
        [JsonPropertyName("RefNum")]
        public string? RefNum { get; set; }

        [JsonPropertyName("FtId")]
        public string? FtId { get; set; }

        [JsonPropertyName("SystemTrace")]
        public string? SystemTrace { get; set; }
    }

    public class PvcbDepositResponseCreditorAccount
    {
        [JsonPropertyName("SourceNumber")]
        public string? SourceNumber { get; set; }

        [JsonPropertyName("SourceType")]
        public string? SourceType { get; set; }

        [JsonPropertyName("SourceName")]
        public string? SourceName { get; set; }
    }
}
