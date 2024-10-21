using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopDev.Chat.ApplicationServices.ChatModule.Dtos
{
	public class ConversationDto
	{
		public string Id { get; set; } = null!; // ID của cuộc trò chuyện
		public required string ConversationName { get; set; }  // ID cuộc trò chuyện
		public List<ChatParticipantDto> Participants { get; set; } = []; // Danh sách người tham gia
		public List<MessageDto> Messages { get; set; } = []; // Danh sách người tham gia
		public DateTime? LastMessageTime { get; set; } // Thời gian tin nhắn cuối cùng
		public string? LastMessage { get; set; } // Nội dung tin nhắn cuối cùng
		public string? LastMessUserName { get; set; } // Tên người gửi tin nhắn cuối cùng
	}

	public class ChatParticipantDto
	{
		public string UserId { get; set; } = null!; // ID người dùng
		public DateTime JoinedAt { get; set; } // Ngày tham gia
		public string Username { get; set; } = null!; // Tên người dùng
		public string ProfilePictureUri { get; set; } = null!; // URL ảnh đại diện
		public int Status { get; set; } // Trạng thái người dùng (online, offline)
		public int NumberOfUnRead { get; set; } // Số lượng tin nhắn chưa đọc
		public DateTime? LastMessageTime { get; set; } // Thời gian tin nhắn cuối cùng
		public string? LastMessage { get; set; } // Nội dung tin nhắn cuối cùng
		public bool IsPrivateDeleted { get; set; } // Trạng thái đã xóa tin nhắn riêng
		public string? LastMessUserName { get; set; } // Tên người gửi tin nhắn cuối cùng
	}
}
