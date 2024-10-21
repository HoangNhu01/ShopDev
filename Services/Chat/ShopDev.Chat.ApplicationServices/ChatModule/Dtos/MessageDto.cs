namespace ShopDev.Chat.ApplicationServices.ChatModule.Dtos
{
	public class MessageDto
	{
		public string Id { get; set; } = null!; // ID của tin nhắn
		public string ConversationId { get; set; } = null!; // ID cuộc trò chuyện
		public required string ConversationName { get; set; }  // ID cuộc trò chuyện
		public string SenderId { get; set; } = null!; // ID người gửi
		public string? Content { get; set; } // Nội dung tin nhắn
		public bool IsPin { get; set; } // Trạng thái ghim tin nhắn
		public DateTime Timestamp { get; set; } // Thời gian gửi
		public int Type { get; set; } // Loại tin nhắn
		public int ContentType { get; set; } // Kiểu nội dung
		public bool IsRecall { get; set; } // Có được thu hồi
		public List<ReaderDto> Readers { get; set; } = []; // Danh sách người đã đọc
		public List<ReactionDto> Reactions { get; set; } = []; // Danh sách người đã reaction
		public List<ChatDocumentDto>? Documents { get; set; } // Danh sách tài liệu
	}

	public class ReactionDto
	{
		public string UserId { get; set; } = null!; // ID người dùng
		public string IconContent { get; set; } = null!; // Nội dung biểu tượng
		public int ReactCount { get; set; } // Số lượng react
	}

	public class ReaderDto
	{
		public string UserId { get; set; } = null!; // ID người dùng
		public DateTime ReadAt { get; set; } // Thời điểm đọc
		public bool IsPrivateDeleted { get; set; } // Trạng thái đã xóa tin nhắn riêng
	}

	public class ChatDocumentDto
	{
		public string? Url { get; set; } // URL tài liệu
		public string? S3Key { get; set; } // Khóa S3 của tài liệu
		public string? Link { get; set; } // Link tài liệu
	}
}
