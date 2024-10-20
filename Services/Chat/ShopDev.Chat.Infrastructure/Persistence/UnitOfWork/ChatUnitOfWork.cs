using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ShopDev.Chat.Infrastructure.Repositories.Abstracts;

namespace ShopDev.Chat.Infrastructure.Persistence.UnitOfWork
{
    public class ChatUnitOfWork : IChatUnitOfWork
    {
        private IClientSessionHandle? _session;
        private readonly IMongoClient _client;
        private bool _disposed;

        public IConversationRepository ConversationRepository { get; }

        public ChatUnitOfWork(
            IMongoClient client,
            IConversationRepository conversationRepository,
            IOptions<MongoDBSettings> settings
        )
        {
            _client = client;
            ConversationRepository = conversationRepository;
        }

        public void BeginTransaction()
        {
            _session = _client.StartSession();
            _session.StartTransaction();
        }

        public async Task CommitAsync()
        {
            if (_session is not null)
            {
                try
                {
                    await _session.CommitTransactionAsync();
                }
                catch (Exception)
                {
                    await _session.AbortTransactionAsync();
                    throw;
                }
            }
        }

        public async Task AbortAsync()
        {
            if (_session != null)
            {
                await _session.AbortTransactionAsync();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _session?.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
