using Microsoft.VisualBasic;
using MongoDB.Driver;
using ShopDev.Chat.Domain.Chats;
using ShopDev.Chat.Infrastructure.Repository;

namespace ShopDev.Chat.Infrastructure.Repositories.Abstracts;

public interface IMessageRepository : IRepository<Message, FilterDefinition<Message>>
{
    /// <summary>
    /// Danh sách tin nhắn
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    Task<PagingResult<Message>> GetPagingAsync(string chatId, PagingRequestBaseLimitOffset filter);

    /// <summary>
    /// Tìm theo Id
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<Message?> GetFirstOrDefaultAsync(string id);

    /// <summary>
    /// Xóa 1 đoạn chat
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(string userId, string id);
    Task<Message> CreateMessageAsync(Message entity);
}
