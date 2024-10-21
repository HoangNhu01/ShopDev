using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using ShopDev.Chat.Domain.Users;
using ShopDev.Chat.Infrastructure.Repositories.Abstracts;
using ShopDev.Chat.Infrastructure.Repository;

namespace ShopDev.Chat.Infrastructure.Repositories.Implements;

public class UserRepository
    : Repository<User, FilterDefinition<User>, UpdateDefinition<User>>,
        IUserRepository
{
    public UserRepository(
        ILogger<UserRepository> logger,
        IHttpContextAccessor httpContextAccessor,
        IMongoDatabase database
    )
        : base(logger, httpContextAccessor, database, nameof(User).ToLower()) { }

    public override async Task SetIndex()
    {
        var indexKeys = Builders<User>
            .IndexKeys.Ascending(x => x.Username)
            .Ascending(x => x.Deleted);
        await _collection.Indexes.CreateOneAsync(new CreateIndexModel<User>(indexKeys));
    }

    public async Task<PagingResult<User>> GetPagingAsync(PagingRequestBaseLimitOffset filter)
    {
        var filters = Builders<User>.Filter.And(
            Builders<User>.Filter.Eq(field: x => x.Deleted, false)
        );

        var totalItems = await _collection.CountDocumentsAsync(filters);
        var items = await _collection
            .Find(filters)
            .Skip(filter.OffSet)
            .Limit(filter.Limit)
            .ToListAsync();

        return new PagingResult<User> { Items = items, TotalItems = totalItems };
    }

    public async Task<User?> GetFirstOrDefaultAsync(string userId)
    {
        var filter = Builders<User>.Filter.And(
            Builders<User>.Filter.Eq(field: x => x.Id, userId),
            Builders<User>.Filter.Eq(field: x => x.Deleted, false)
        );
        return await GetFirstOrDefaultAsync(filter);
    }

    public async Task DeleteAsync(string userId)
    {
        var filter = Builders<User>.Filter.And(
            Builders<User>.Filter.Eq(field: x => x.Id, userId),
            Builders<User>.Filter.Eq(field: x => x.Deleted, false)
        );
        await DeleteAsync(filter);
    }

    public async Task<IEnumerable<User>> GetListAsync(List<string> userIds)
    {
        var filter = Builders<User>.Filter.And(
            Builders<User>.Filter.In(field: x => x.Id, userIds),
            Builders<User>.Filter.Eq(field: x => x.Deleted, false)
        );
        return await _collection.Find(filter).ToListAsync();
    }
}
