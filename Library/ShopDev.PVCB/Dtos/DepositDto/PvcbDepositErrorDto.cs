using System.Text.Json.Serialization;
using CR.PVCB.Dtos.PvcbBaseDto;

namespace CR.PVCB.Dtos.DepositDto
{
    /// <summary>
    /// Thông tin thêm (nếu có) Ressponse lỗi từ Deposit
    /// </summary>
    public class PvcbDepositErrorMoreInfor
    {
        [JsonPropertyName("refNum")]
        public string? RefNum { get; set; }

        [JsonPropertyName("ftId")]
        public string? FtId { get; set; }

        [JsonPropertyName("systemTrace")]
        public string? SystemTrace { get; set; }
    }

    /// <summary>
    /// Ressponse lỗi từ Deposit
    /// </summary>
    public class PvcbDepositErrorDto : PvcbBaseError
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("detail")]
        public string? Detail { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("moreInfor")]
        public PvcbDepositErrorMoreInfor? MoreInfor { get; set; }
    }
}
