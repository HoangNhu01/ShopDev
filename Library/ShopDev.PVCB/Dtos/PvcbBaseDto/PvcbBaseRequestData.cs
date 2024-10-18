using System.Text.Json.Serialization;

namespace CR.PVCB.Dtos.PvcbBaseDto
{
    public class PvcbBaseRequestData
    {
        /// <summary>
        /// Mã giao dịch từ 6 - 12 ký tự
        /// </summary>
        [JsonPropertyName("TransId")]
        public required string TransId { get; set; }

        /// <summary>
        /// Thời gian giao dịch
        /// </summary>
        [JsonPropertyName("DateTime")]
        public required string DateTime { get; set; }
    }
}
