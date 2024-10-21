namespace ShopDev.Chat.ApplicationServices.ChatModule.Dtos
{
	public class MessageCreateDto
	{
		public string Id { get; set; } = null!; // ID của tin nhắn
		public string ChatId { get; set; } = null!; // ID cuộc trò chuyện
		public required string ConversationName { get; set; }  // ID cuộc trò chuyện
		public string SenderId { get; set; } = null!; // ID người gửi
		public string? Content { get; set; } // Nội dung tin nhắn
		public bool IsPin { get; set; } // Trạng thái ghim tin nhắn
		public int Type { get; set; } // Loại tin nhắn
		public int ContentType { get; set; } // Kiểu nội dung
		public List<ChatDocumentCreateDto>? Documents { get; set; } // Danh sách tài liệu
	}

	public class ChatDocumentCreateDto
	{
		public string? Url { get; set; } // URL tài liệu
		public string? S3Key { get; set; } // Khóa S3 của tài liệu
		public string? Link { get; set; } // Link tài liệu
	}
}
