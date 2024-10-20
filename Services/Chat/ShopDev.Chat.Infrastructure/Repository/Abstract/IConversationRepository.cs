using Microsoft.VisualBasic;
using MongoDB.Driver;
using ShopDev.Chat.Domain.Chats;
using ShopDev.Chat.Infrastructure.Repository;

namespace ShopDev.Chat.Infrastructure.Repositories.Abstracts;

public interface IConversationRepository
    : IRepository<Conversation, FilterDefinition<Conversation>>
{
    /// <summary>
    /// Danh sách cuộc trò chuyện
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    Task<PagingResult<Conversation>> GetPagingAsync(string userId, PagingRequestBaseLimitOffset filter);

    /// <summary>
    /// Tìm theo Id
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<Conversation?> GetFirstOrDefaultAsync(string userId, string id);

    /// <summary>
    /// Xóa 1 đoạn chat
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteAsync(string userId, string id);
}
