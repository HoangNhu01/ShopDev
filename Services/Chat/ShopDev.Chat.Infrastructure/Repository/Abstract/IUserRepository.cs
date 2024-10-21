using MongoDB.Driver;
using ShopDev.Chat.Domain.Chats;
using ShopDev.Chat.Domain.Users;
using ShopDev.Chat.Infrastructure.Repository;

namespace ShopDev.Chat.Infrastructure.Repositories.Abstracts;

public interface IUserRepository : IRepository<User, FilterDefinition<User>>
{
    /// <summary>
    /// Danh sách user phân trang
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    Task<PagingResult<User>> GetPagingAsync(PagingRequestBaseLimitOffset filter);

    /// <summary>
    /// Danh sách user
    /// </summary>
    /// <param name="userIds"></param>
    /// <returns></returns>
    Task<IEnumerable<User>> GetListAsync(List<string> userIds);

    /// <summary>
    /// Tìm theo Id
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<User?> GetFirstOrDefaultAsync(string userId);

    /// <summary>
    /// Xóa 1 user
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task DeleteAsync(string userId);
}
