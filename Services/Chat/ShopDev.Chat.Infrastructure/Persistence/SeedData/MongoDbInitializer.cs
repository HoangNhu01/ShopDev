using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ShopDev.Chat.Infrastructure.Persistence.UnitOfWork;

namespace ShopDev.Chat.Infrastructure.Persistence.SeedData
{
    public partial class MongoDbInitializer
    {
        private readonly ILogger _logger;
        private readonly IChatUnitOfWork _chatUnitOfWork;

        public MongoDbInitializer(
            ILogger<MongoDbInitializer> logger,
            IServiceProvider serviceProvider
        )
        {
            _logger = logger;
            using var scope = serviceProvider.CreateScope();
            _chatUnitOfWork = scope.ServiceProvider.GetRequiredService<IChatUnitOfWork>();
        }

        public async Task SeedAsync()
        {
            _logger.LogInformation("{0}->{1}", GetType().FullName, nameof(SeedAsync));
            //_notiUnitOfWork.BeginTransaction();
            await SeedConversation();
            //await _notiUnitOfWork.CommitAsync();
        }
    }
}
