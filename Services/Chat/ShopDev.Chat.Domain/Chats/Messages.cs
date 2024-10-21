using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ShopDev.Abstractions.EntitiesBase.Interfaces;
using ShopDev.EntitiesBase.Base;

namespace ShopDev.Chat.Domain.Chats
{
    public class Message: IFullAudited, IEntity<string>
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; }  // ID của tin nhắn

        [BsonRepresentation(BsonType.ObjectId)]
        public required string ConversationId { get; set; }  // ID cuộc trò chuyện
        public string? ConversationName { get; set; }  // ID cuộc trò chuyện

        [BsonRepresentation(BsonType.ObjectId)]
        public required string SenderId { get; set; }  // ID người gửi

        public string? Content { get; set; }  // Nội dung tin nhắn
        public bool IsPin { get; set; }
        public DateTime Timestamp { get; set; }  // Thời gian gửi
        public int Type { get; set; }  // Loại tin nhắn
        public int ContentType { get; set; }
        /// <summary>
        /// CÓ đưuọc thu hồi
        /// </summary>
        public bool IsRecall { get; set; }
        public List<Reader> Readers { get; set; } = [];  // Danh sách người đã đọc
        public List<Reaction> Reactions { get; set; } = [];  // Danh sách người đã reaction
        public List<ChatDocument>? Documents { get; set; }  // Danh sách tài liệu

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
	public class Reaction 
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public required string UserId { get; set; }  // ID người dùng
        public required string IconContent { get; set; }
        public int ReactCount { get; set; }
    }
    public class Reader
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public required string UserId { get; set; }  // ID người dùng
        public DateTime ReadAt { get; set; }  // Thời điểm đọc
        /// <summary>
        /// Xóa tin nhắn chỉ với user
        /// </summary>
        public bool IsPrivateDeleted { get; set; }
    }

    public class ChatDocument
    {
        public string? Url { get; set; }  // URL tài liệu
        public string? S3Key { get; set; }  // Khóa s3 của tài liệu
        public string? Link { get; set; }  // Link
    }
}
