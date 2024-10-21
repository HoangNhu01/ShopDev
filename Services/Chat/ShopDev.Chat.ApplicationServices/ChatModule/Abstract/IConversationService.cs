using ShopDev.Chat.ApplicationServices.ChatModule.Dtos;

namespace ShopDev.Chat.ApplicationServices.ChatModule.Abstract
{
    public interface IConversationService
    {
        Task<ConversationDto> Create(List<string> userIds);

		Task<ConversationDto> FindById(string id);
		Task<PagingResult<ConversationDto>> FindAll(PagingConversationDto input);

	}
}
