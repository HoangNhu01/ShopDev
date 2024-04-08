using System.Text.Json.Serialization;

namespace CR.PVCB.Dtos.EkycDtos
{
    /// <summary>
    /// Thông tin trả về truy vấn thông tin Ekyc
    /// </summary>
    public class PvcbEkycResponseDto
    {
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("data")]
        public string? Data { get; set; }
    }

    /// <summary>
    /// Thông tin trả về truy vấn thông tin Ekyc
    /// </summary>
    public class PvcbEkycResponseInformationDto
    {
        /// <summary>
        /// Số tài khoản
        /// </summary>
        public string? AccountNumber { get; set; }

        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Ngày tháng năm sinh
        /// </summary>
        public string? Dob { get; set; }

        /// <summary>
        /// Giới tính
        /// </summary>
        public string? Gender { get; set; }

        /// <summary>
        /// Ngày cấp GTTT
        /// </summary>
        public string? LegalIssueDate { get; set; }

        /// <summary>
        /// Ngày hết hạn GTTT
        /// </summary>
        public string? LegalExpDate { get; set; }

        /// <summary>
        /// Số GTTT
        /// </summary>
        public required string LegalId { get; set; }

        /// <summary>
        /// Nơi cấp GTTT
        /// </summary>
        public string? LegalPlace { get; set; }

        /// <summary>
        /// Nơi ở hiện tại
        /// </summary>
        public string? Domicile { get; set; }

        /// <summary>
        /// Quê quán
        /// </summary>
        public string? Resident { get; set; }

        /// <summary>
        /// Mã định danh của user do tác truyền vào
        /// </summary>
        public string? UserIdPartner { get; set; }

        /// <summary>
        /// Ảnh form thông tin GTTT của KH
        /// </summary>
        public string? FormInfoImgUrl { get; set; }

        /// <summary>
        /// Ảnh GTTT mặt trước
        /// </summary>
        public string? LegalFrontImgUrl { get; set; }

        /// <summary>
        /// Ảnh GTTT mặt sau
        /// </summary>
        public string? LegalBackImgUrl { get; set; }

        /// <summary>
        /// Video eKYC
        /// </summary>
        public string? VideoUrl { get; set; }

        /// <summary>
        /// Các ảnh khuôn mặt eKYC
        /// </summary>
        public string[]? FaceImgUrls { get; set; }
    }
}
