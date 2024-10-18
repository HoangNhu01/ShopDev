using System.Text.Json.Serialization;
using CR.PVCB.Constants;
using CR.PVCB.Dtos.PvcbBaseDto;

namespace CR.PVCB.Dtos.InquiryDtos
{
    /// <summary>
    /// Request Truy vấn tài khoản thụ hưởng
    /// </summary>
    public class PvcbInquiryRequestDto
    {
        [JsonPropertyName("Data")]
        public required PvcbInquiryRequestData Data { get; set; }

        [JsonPropertyName("Risk")]
        public required PvcbBaseRequestRisk? Risk { get; set; }
    }

    /// <summary>
    /// Thông tin tài khoản
    /// </summary>
    public class PvcbInquityRequestCreditorAccount
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

    public class PvcbInquiryRequestData : PvcbBaseRequestData
    {
        /// <summary>
        /// Thông tin tài khoản
        /// </summary>
        [JsonPropertyName("CreditorAccount")]
        public required PvcbInquityRequestCreditorAccount CreditorAccount { get; set; }
    }
}
