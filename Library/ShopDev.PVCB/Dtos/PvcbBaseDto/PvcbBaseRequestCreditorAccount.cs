using System.Text.Json.Serialization;

namespace CR.PVCB.Dtos.PvcbBaseDto
{
    /// <summary>
    /// Base request cho các request của Pvcb
    /// </summary>
    public class PvcbBaseRequestCreditorAccount
    {
        /// <summary>
        /// Số tài khoản/số thẻ ngân hàng thụ hưởng cần truy vấn
        /// </summary>
        [JsonPropertyName("SourceNumber")]
        public required string SourceNumber { get; set; }

        /// <summary>
        /// Truyền PAN: nếu là số thẻ / Hoặc ACC: nếu là số tài khoản
        /// <see cref="SourceTypes"/>
        /// </summary>
        [JsonPropertyName("SourceType")]
        public required string SourceType { get; set; }
    }
}
