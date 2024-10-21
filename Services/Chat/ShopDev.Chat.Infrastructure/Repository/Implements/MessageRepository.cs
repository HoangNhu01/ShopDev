using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using ShopDev.Chat.Domain.Chats;
using ShopDev.Chat.Domain.Users;
using ShopDev.Chat.Infrastructure.Repositories.Abstracts;
using ShopDev.Chat.Infrastructure.Repository;
using ShopDev.Utils.DataUtils;

namespace ShopDev.Chat.Infrastructure.Repositories.Implements;

public class MessageRepository
    : Repository<Message, FilterDefinition<Message>, UpdateDefinition<Message>>,
        IMessageRepository
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IUserRepository _userRepository;

    public MessageRepository(
        ILogger<MessageRepository> logger,
        IHttpContextAccessor httpContextAccessor,
        IMongoDatabase database,
        IUserRepository userRepository,
        IConversationRepository conversationRepository
    )
        : base(logger, httpContextAccessor, database, nameof(Message).ToLower())
    {
        _userRepository = userRepository;
        _conversationRepository = conversationRepository;
    }

    public override async Task SetIndex()
    {
        var indexKeys = Builders<Message>
            .IndexKeys.Ascending(x => x.SenderId)
            .Ascending(x => x.Deleted);
        await _collection.Indexes.CreateOneAsync(new CreateIndexModel<Message>(indexKeys));
    }

    public async Task<PagingResult<Message>> GetPagingAsync(
        string chatId,
        PagingRequestBaseLimitOffset filter
    )
    {
        var filters = Builders<Message>.Filter.And(
            Builders<Message>.Filter.Eq(field: x => x.Deleted, false),
            Builders<Message>.Filter.Eq(field: x => x.ConversationId, chatId)
        );

        var totalItems = await _collection.CountDocumentsAsync(filters);
        var items = await _collection
            .Find(filters)
            .SortByDescending(x => x.CreatedDate)
            .Skip(filter.OffSet)
            .Limit(filter.Limit)
            .ToListAsync();
        return new PagingResult<Message> { Items = items, TotalItems = totalItems };
    }

    public async Task<Message?> GetFirstOrDefaultAsync(string id)
    {
        var filter = Builders<Message>.Filter.And(
            Builders<Message>.Filter.Eq(x => x.Id, id),
            Builders<Message>.Filter.Eq(field: x => x.Deleted, false)
        );
        return await GetFirstOrDefaultAsync(filter);
    }

    public Task<bool> DeleteAsync(string userId, string id)
    {
        throw new NotImplementedException();
    }

    public async Task<Message> CreateMessageAsync(Message entity)
    {
        var conversationName = await _conversationRepository.GetConversationName(
            entity.SenderId,
            entity.ConversationId
        );
        entity.ConversationName = conversationName;
        return await CreateAsync(entity);
    }
}
