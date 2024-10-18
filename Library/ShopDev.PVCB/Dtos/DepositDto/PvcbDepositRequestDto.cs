using System.Text.Json.Serialization;
using CR.PVCB.Dtos.PvcbBaseDto;

namespace CR.PVCB.Dtos.DepositDto
{
    /// <summary>
    /// Request deposit
    /// </summary>
    public class PvcbDepositRequestDto
    {
        [JsonPropertyName("Data")]
        public required PvcbDepositRequestData Data { get; set; }

        [JsonPropertyName("Risk")]
        public required PvcbDepositRisk Risk { get; set; }
    }

    public class PvcbDepositRequestCreditorAccount : PvcbBaseRequestCreditorAccount
    {
        /// <summary>
        /// Tên chủ tài khoản thụ hưởng
        /// </summary>
        [JsonPropertyName("SourceName")]
        public required string SourceName { get; set; }
    }

    public class PvcbDepositRequestData : PvcbBaseRequestData
    {
        [JsonPropertyName("CreditorAccount")]
        public PvcbDepositRequestCreditorAccount? CreditorAccount { get; set; }

        [JsonPropertyName("InstructedAmount")]
        public PvcbDepositRequestInstructedAmount? InstructedAmount { get; set; }
    }

    public class PvcbDepositRequestInstructedAmount
    {
        /// <summary>
        /// Số tiền chuyển khoản
        /// </summary>
        [JsonPropertyName("Amount")]
        public required int Amount { get; set; }
    }

    public class PvcbDepositRisk : PvcbBaseRequestRisk
    {
        /// <summary>
        /// Nội dung chuyển khoản, tiếng việt không dấu, không chứ ký tự đặc biệt, <=210 ký tự
        /// </summary>
        [JsonPropertyName("TransDesc")]
        public string? TransDesc { get; set; }
    }
}
