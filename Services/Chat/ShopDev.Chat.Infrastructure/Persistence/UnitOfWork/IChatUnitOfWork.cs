using ShopDev.Chat.Infrastructure.Repositories.Abstracts;

namespace ShopDev.Chat.Infrastructure.Persistence.UnitOfWork
{
    public interface IChatUnitOfWork : IDisposable
    { 
        IConversationRepository ConversationRepository { get; }
        void BeginTransaction();
        Task CommitAsync();
        Task AbortAsync();
    }
}
