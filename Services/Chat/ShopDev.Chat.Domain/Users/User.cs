using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ShopDev.Abstractions.EntitiesBase.Interfaces;
using ShopDev.Constants.Database;
using ShopDev.Constants.Users;
using ShopDev.EntitiesBase.AuthorizationEntities;

namespace ShopDev.Chat.Domain.Users
{
    /// <summary>
    /// User
    /// </summary>
    public class User : IFullAudited
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; } // ID của người dùng

        public required string Username { get; set; } // Tên người dùng
        public required string Email { get; set; } // Địa chỉ email
        public DateTime? LastLogin { get; set; } // Ngày đăng nhập gần nhất
        public required string ProfilePicture { get; set; } // URL ảnh đại diện
        public required string Status { get; set; } // Trạng thái người dùng (online, offline)
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
