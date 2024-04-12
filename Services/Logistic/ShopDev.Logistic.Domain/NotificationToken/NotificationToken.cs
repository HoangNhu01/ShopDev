using ShopDev.Authentication.Domain.Users;
using ShopDev.Constants.Database;
using Microsoft.EntityFrameworkCore;

namespace ShopDev.Authentication.Domain.AuthToken
{
    [Table(nameof(NotificationToken), Schema = DbSchemas.Default)]
    [Index(nameof(UserId), Name = $"IX_{nameof(NotificationToken)}")]
    public class NotificationToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(128)]
        [Unicode(false)]
        public string? FcmToken { get; set; }

        [MaxLength(128)]
        [Unicode(false)]
        public string? ApnsToken { get; set; }
        public int UserId { get; set; }
        public User User { get; } = null!;
    }
}
