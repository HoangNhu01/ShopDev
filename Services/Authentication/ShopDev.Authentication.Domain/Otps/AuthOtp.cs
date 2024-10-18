using ShopDev.Authentication.Domain.Users;
using ShopDev.Constants.Database;
using Microsoft.EntityFrameworkCore;

namespace ShopDev.Authentication.Domain.Otps
{
    [Table(nameof(AuthOtp), Schema = DbSchemas.Default)]
    [Index(
        nameof(OtpCode),
        nameof(ExpireTime),
        nameof(IsUsed),
        nameof(UserId),
        Name = $"IX_{nameof(AuthOtp)}"
    )]
    public class AuthOtp
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Otp code được mã hóa
        /// </summary>
        [MaxLength(128)]
        [Unicode(false)]
        public required string OtpCode { get; set; }

        /// <summary>
        /// Thời gian hết hạn
        /// </summary>
        public DateTime ExpireTime { get; set; }

        /// <summary>
        /// Check đã được dùng
        /// </summary>
        public bool IsUsed { get; set; }

        /// <summary>
        /// Opt cho người nào
        /// </summary>
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
