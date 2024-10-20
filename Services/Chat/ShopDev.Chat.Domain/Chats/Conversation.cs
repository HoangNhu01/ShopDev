using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopDev.Abstractions.EntitiesBase.Interfaces;
using ShopDev.EntitiesBase.Base;

namespace ShopDev.Chat.Domain.Chats
{
    public class Conversation : IFullAudited, IEntity<string>
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; }  // ID của cuộc trò chuyện    

        public List<ChatParticipant> Participants { get; set; } = [];  // Danh sách người tham gia
        /// <summary>
        /// Thời gian tin nhắn cuối cùng
        /// </summary>
        public DateTime? LastMessageTime { get; set; }
        public required string LastMessage { get; set; }
        public string? LastMessUserName { get; set; }
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

    public class ChatParticipant
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public required string UserId { get; set; }  // ID người dùng
        public DateTime JoinedAt { get; set; }  // Ngày tham gia
        public required string Username { get; set; }  // Tên người dùng
        public required string ProfilePictureUri { get; set; }  // URL ảnh đại diện
        public int Status { get; set; }  // Trạng thái người dùng (online, offline)
        /// <summary>
        /// Số lượng tin nhắn chưa đọc
        /// </summary>
        public int NumberOfUnRead { get; set; }
        /// <summary>
        /// Thời gian tin nhắn cuối cùng
        /// </summary>
        public DateTime LastMessageTime { get; set; }
        public required string LastMessage { get; set; }  // Tên người dùng
        public bool IsPrivateDeleted { get; set; }
        public string? LastMessUserName { get; set; }
    }
}
