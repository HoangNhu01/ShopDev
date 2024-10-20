using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using ShopDev.Chat.Domain.Chats;
using ShopDev.Chat.Infrastructure.Repositories.Abstracts;
using ShopDev.Chat.Infrastructure.Repository;

namespace ShopDev.Chat.Infrastructure.Repositories.Implements;

public class ConversationRepository
    : Repository<Conversation, FilterDefinition<Conversation>, UpdateDefinition<Conversation>>,
        IConversationRepository
{
    public ConversationRepository(
        ILogger<ConversationRepository> logger,
        IHttpContextAccessor httpContextAccessor,
        IMongoDatabase database
    )
        : base(logger, httpContextAccessor, database, nameof(Conversation).ToLower()) { }

    public override async Task SetIndex()
    {
        var indexKeys = Builders<Conversation>.IndexKeys.Ascending(x => x.Participants.Select(x => x.UserId));
        await _collection.Indexes.CreateOneAsync(new CreateIndexModel<Conversation>(indexKeys));
    }

    public async Task<PagingResult<Conversation>> GetPagingAsync(
        string userId,
        PagingRequestBaseLimitOffset filter
    )
    {
        var filters = Builders<Conversation>.Filter.And(
            Builders<Conversation>.Filter.ElemMatch(
                x => x.Participants,
                Builders<ChatParticipant>.Filter.Eq(field: x => x.UserId, userId)
            ),
            Builders<Conversation>.Filter.Eq(field: x => x.Deleted, false)
        );

        var totalItems = await _collection.CountDocumentsAsync(filters);
        var items = await _collection
            .Find(filters)
            .SortByDescending(x => x.LastMessageTime)
            .ThenByDescending(x => x.CreatedDate)
            .Skip(filter.OffSet)
            .Limit(filter.Limit)
            .ToListAsync();

        return new PagingResult<Conversation> { Items = items, TotalItems = totalItems };
    }

    public async Task<Conversation?> GetFirstOrDefaultAsync(string userId, string id)
    {
        var filter = Builders<Conversation>.Filter.And(
            Builders<Conversation>.Filter.Eq(x => x.Id, id)
        );
        return await GetFirstOrDefaultAsync(filter);
    }

    public Task DeleteAsync(string userId, string id)
    {
        throw new NotImplementedException();
    }
}
