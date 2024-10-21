using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using ShopDev.Authentication.Domain.Users;
using ShopDev.Chat.Domain.Chats;
using ShopDev.Chat.Domain.Users;
using ShopDev.Chat.Infrastructure.Repositories.Abstracts;
using ShopDev.Chat.Infrastructure.Repository;
using ShopDev.Utils.DataUtils;
using User = ShopDev.Chat.Domain.Users.User;

namespace ShopDev.Chat.Infrastructure.Repositories.Implements;

public class ConversationRepository
    : Repository<Conversation, FilterDefinition<Conversation>, UpdateDefinition<Conversation>>,
        IConversationRepository
{
    private readonly IUserRepository _userRepository;

    public ConversationRepository(
        ILogger<ConversationRepository> logger,
        IHttpContextAccessor httpContextAccessor,
        IMongoDatabase database,
        IUserRepository userRepository
    )
        : base(logger, httpContextAccessor, database, nameof(Conversation).ToLower())
    {
        _userRepository = userRepository;
    }

    public override async Task SetIndex()
    {
        var indexKeys = Builders<Conversation>.IndexKeys.Ascending(x =>
            x.Participants.Select(x => x.UserId)
        );
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
            Builders<Conversation>.Filter.Eq(x => x.Id, id),
            Builders<Conversation>.Filter.ElemMatch(
                x => x.Participants,
                Builders<ChatParticipant>.Filter.Eq(field: x => x.UserId, userId)
            ),
            Builders<Conversation>.Filter.Eq(field: x => x.Deleted, false)
        );
        return await GetFirstOrDefaultAsync(filter);
    }

    public async Task<bool> DeleteAsync(string userId, string id)
    {
        var filter = Builders<Conversation>.Filter.And(
            Builders<Conversation>.Filter.Eq(x => x.Id, id),
            Builders<Conversation>.Filter.ElemMatch(
                x => x.Participants,
                Builders<ChatParticipant>.Filter.Eq(field: x => x.UserId, userId)
            ),
            Builders<Conversation>.Filter.Eq(field: x => x.Deleted, false)
        );
        var conversation = await GetFirstOrDefaultAsync(filter);
        if (conversation is null || !conversation.Participants.Exists(x => x.UserId == userId))
        {
            return default;
        }
        conversation.Participants.Find(x => x.UserId == userId)!.IsPrivateDeleted = true;
        return (await _collection.ReplaceOneAsync(filter, conversation)).IsModifiedCountAvailable;
    }

    public async Task<Conversation> GetOrCreateChatAsync(List<string> userIds)
    {
        // Kiểm tra đoạn chat đã tồn tại với cùng người tham gia và loại chat
        var filter = Builders<Conversation>.Filter.And(
            Builders<Conversation>.Filter.Eq(c => c.Deleted, false),
            Builders<Conversation>.Filter.All(c => c.Participants.Select(p => p.UserId), userIds)
        );
        var existingChat = await GetFirstOrDefaultAsync(filter);
        if (existingChat is not null)
        {
            // Trả về đoạn chat nếu đã tồn tại
            return existingChat;
        }
        DateTime now = DateTimeUtils.GetDate();
        IEnumerable<User> users = await _userRepository.GetListAsync(userIds);
        // Nếu chưa tồn tại, tạo mới đoạn chat
        var newChat = new Conversation
        {
            Participants =
            [
                .. users.Select(x => new ChatParticipant
			{
				UserId = x.Id,
				JoinedAt = now,
				IsPrivateDeleted = false,
				LastMessage = null,
				LastMessageTime = now,
				ProfilePictureUri = x.ProfilePicture,
				Username = x.Username,
			})
            ],
            LastMessage = null,
            LastMessageTime = now,
            LastMessUserName = null,
            ConversationName = string.Join(',', users.Select(x => x.Username))
        };

        await CreateAsync(newChat);
        return newChat;
    }

    public async Task<ConversationLookup> GetChatWithMessagesAsync(string userId, string id)
    {
        var filter = Builders<Conversation>.Filter.And(
            Builders<Conversation>.Filter.Eq(x => x.Id, id),
            Builders<Conversation>.Filter.ElemMatch(
                x => x.Participants,
                Builders<ChatParticipant>.Filter.Eq(field: x => x.UserId, userId)
            ),
            Builders<Conversation>.Filter.Eq(field: x => x.Deleted, false)
        );
        var messageCollection = _database.GetCollection<Message>(nameof(Message).ToLower());
        var pipeline = _collection
            .Aggregate()
            .Match(filter)
            .Lookup<Conversation, Message, ConversationLookup>(
                messageCollection, // Collection chứa các message
                x => x.Id, // Khóa chính của chat
                x => x.ConversationId, // Khóa ngoại trong message
                x => x.ListMessages // Gán kết quả vào trường Messages
            );

        return await pipeline.FirstOrDefaultAsync();
    }

    public async Task<string?> GetConversationName(string userId, string id)
    {
        var filter = Builders<Conversation>.Filter.And(
            Builders<Conversation>.Filter.Eq(x => x.Id, id),
            Builders<Conversation>.Filter.ElemMatch(
                x => x.Participants,
                Builders<ChatParticipant>.Filter.Eq(field: x => x.UserId, userId)
            ),
            Builders<Conversation>.Filter.Eq(field: x => x.Deleted, false)
        );
        if ((await ProjectionOneAsync(filter, x => x.ConversationName)) is string conversationName)
        {
            return conversationName;
        }
        return null;
    }
}
