using System.Text.Json.Serialization;
using CR.PVCB.Dtos.PvcbBaseDto;

namespace CR.PVCB.Dtos.InquiryDtos
{
    /// <summary>
    /// Response lỗi khi truy vấn tài khoản ngân hàng từ PVCB
    /// </summary>
    public class PvcbInquiryErrorDto : PvcbBaseError
    {
        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}
