using ShopDev.Constants.Database;
using ShopDev.Constants.Users;
using Microsoft.EntityFrameworkCore;
using ShopDev.EntitiesBase.AuthorizationEntities;

namespace ShopDev.Authentication.Domain.Users
{
    /// <summary>
    /// User
    /// </summary>
    [Table(nameof(User), Schema = DbSchemas.Default)]
    [Index(
        nameof(Username),
        nameof(CustomerId),
        nameof(CreatedDate),
        nameof(Status),
        nameof(UserType),
        Name = $"IX_{nameof(User)}"
    )]
    public class User : IUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(128)]
        [Unicode(false)]
        public string Username { get; set; } = null!;

        [Required]
        [MaxLength(128)]
        [Unicode(false)]
        public string Password { get; set; } = null!;

        [MaxLength(256)]
        public string? FullName { get; set; }

        /// <summary>
        /// Loại user <see cref="UserTypes"/>
        /// </summary>
        public int UserType { get; set; }

        /// <summary>
        /// Trạng thái user <see cref="UserStatus"/>
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Thời gian xóa tài khoản theo Status = 3(LOCK)
        /// </summary>
        public DateTime? LockedStatus { get; set; }

        /// <summary>
        /// Mã pin
        /// </summary>
        [MaxLength(128)]
        [Unicode(false)]
        public string? PinCode { get; set; }
        public bool IsTempPin { get; set; }

        /// <summary>
        /// Id của khách hàng bảng chính
        /// </summary>
        public int? CustomerId { get; set; }

        /// <summary>
        /// Có phải là mật khẩu tạm, Yêu cầu thay đổi mật khẩu mới ngay khi đăng nhập
        /// </summary>
        public bool IsPasswordTemp { get; set; }

        /// <summary>
        /// Lần đầu đăng nhập vào App
        /// Mặc định là false. True khi tạo tài khoản trên Cms chọn là mật khẩu tạm !IsPasswordTemp
        /// </summary>
        public bool IsFirstTime { get; set; }

        [MaxLength(256)]
        public string? OperatingSystem { get; set; }

        [MaxLength(256)]
        public string? Browser { get; set; }
        public DateTime? LastLogin { get; set; }

        /// <summary>
        /// Ảnh đại diện
        /// </summary>
        [MaxLength(2048)]
        public string? AvatarImageUri { get; set; }

        /// <summary>
        /// S3 Key
        /// </summary>
        [MaxLength(2024)]
        public string? S3Key { get; set; }

        /// <summary>
        /// requestId Otp từ MeeyPartner
        /// </summary>
        [MaxLength(128)]
        public string? OtpRequestId { get; set; }

        /// <summary>
        /// Thời gian gửi lại Otp khi quá số lần gửi Otp từ MeeyPartner
        /// </summary>
        public DateTime? ResendOtpDate { get; set; }

        public int LoginFailCount { get; set; }
        public DateTime DateTimeLoginFailCount { get; set; }

        /// <summary>
        /// Mã bí mật khi xác nhận quên mật khẩu
        /// </summary>
        [MaxLength(128)]
        public string? SecretPasswordCode { get; set; }

        /// <summary>
        /// Thời gian hết hạn mã bí mật khi xác nhận quên mật khẩu
        /// </summary>
        public DateTime? SecretPasswordExpiryDate { get; set; }

        #region audit
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public int? DeletedBy { get; set; }
        public bool Deleted { get; set; }
        #endregion
    }
}
