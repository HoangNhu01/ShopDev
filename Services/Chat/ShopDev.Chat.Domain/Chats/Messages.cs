using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ShopDev.Chat.Domain.Chats
{
    public class Message
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; }  // ID của tin nhắn

        [BsonRepresentation(BsonType.ObjectId)]
        public required string ChatId { get; set; }  // ID cuộc trò chuyện

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
