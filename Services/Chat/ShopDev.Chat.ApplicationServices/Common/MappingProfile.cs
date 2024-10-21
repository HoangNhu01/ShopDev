using AutoMapper;
using ShopDev.Chat.ApplicationServices.ChatModule.Dtos;
using ShopDev.Chat.Domain.Chats;

namespace ShopDev.Chat.ApplicationServices.Common
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<ConversationDto, Conversation>().ReverseMap();
			CreateMap<MessageDto, Message>().ReverseMap();
			CreateMap<MessageCreateDto, Message>().ReverseMap();

		}
	}
}
