namespace ShopDev.Chat.Infrastructure.Persistence.SeedData;

public partial class MongoDbInitializer
{
    private async Task SeedConversation()
    {
        await _chatUnitOfWork.ConversationRepository.SetIndex();
    }
}
