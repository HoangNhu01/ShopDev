using System.Text.Json.Serialization;

namespace CR.PVCB.Dtos.BankInfoDto
{
    /// <summary>
    /// Response danh sách ngân hàng từ api của Pvcb
    /// </summary>
    public class PvcbBankInfoResponseDto
    {
        [JsonPropertyName("Data")]
        public PvcbData? Data { get; set; }

        [JsonPropertyName("Link")]
        public object? Link { get; set; }

        [JsonPropertyName("Meta")]
        public object? Meta { get; set; }
    }

    public class PvcbBankInfor
    {
        /// <summary>
        /// Id dùng cho (BINId) trong các api khác từ Pvcb
        /// </summary>
        [JsonPropertyName("Id")]
        public string? Id { get; set; }

        [JsonPropertyName("Code")]
        public string? Code { get; set; }

        [JsonPropertyName("ShortName")]
        public string? ShortName { get; set; }

        [JsonPropertyName("FullName")]
        public string? FullName { get; set; }

        [JsonPropertyName("Description")]
        public string? Description { get; set; }
    }

    public class PvcbData
    {
        /// <summary>
        /// Danh sách ngân hàng
        /// </summary>
        [JsonPropertyName("BankInfor")]
        public List<PvcbBankInfor>? BankInfor { get; set; }
    }
}
