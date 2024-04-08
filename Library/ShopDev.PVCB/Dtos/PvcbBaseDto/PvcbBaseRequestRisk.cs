using System.Text.Json.Serialization;

namespace CR.PVCB.Dtos.PvcbBaseDto
{
    /// <summary>
    /// base pvcb request risk
    /// </summary>
    public class PvcbBaseRequestRisk
    {
        /// <summary>
        /// Mã ngân hàng lấy từ trường ID trong danh sách ngân hàng
        /// </summary>
        [JsonPropertyName("BINId")]
        public required string BINId { get; set; }
    }
}
