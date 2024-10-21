using ShopDev.Chat.ApplicationServices.ChatModule.Dtos;

namespace ShopDev.Chat.ApplicationServices.ChatModule.Abstract
{
    public interface IMessageService
    {
        Task<MessageDto> Create(MessageCreateDto input);

        Task<MessageDto> FindById(string id);
        Task<PagingResult<MessageDto>> FindById(string chatId, PagingRequestBaseLimitOffset input);
    }
}
