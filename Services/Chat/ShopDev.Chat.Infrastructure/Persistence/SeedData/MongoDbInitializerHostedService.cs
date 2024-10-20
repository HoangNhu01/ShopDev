using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopDev.Chat.Infrastructure.Persistence.SeedData
{
    public class MongoDbInitializerHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public MongoDbInitializerHostedService(
            ILogger<MongoDbInitializerHostedService> logger,
            IServiceProvider serviceProvider
        )
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("{0}->{1}", GetType().FullName, nameof(StartAsync));
            using var scope = _serviceProvider.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredService<MongoDbInitializer>();
            await seeder.SeedAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("{0}->{1}", GetType().FullName, nameof(StopAsync));
            return Task.CompletedTask;
        }
    }
}
