using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ShopDev.IdentityServerBase.Services
{
    public abstract class AuthWorkerBase<TDbContext> : IHostedService
        where TDbContext : DbContext
    {
        protected readonly IServiceProvider _serviceProvider;

        protected AuthWorkerBase(IServiceProvider serviceProvider) =>
            _serviceProvider = serviceProvider;

        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<TDbContext>();
            await context.Database.EnsureCreatedAsync();

            var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            if (await manager.FindByClientIdAsync("client-test", cancellationToken) is null)
            {
                await manager.CreateAsync(
                    new OpenIddictApplicationDescriptor
                    {
                        ClientId = "client-test",
                        ClientSecret = "12345",
                        DisplayName = "Client test",
                        Permissions =
                        {
                            Permissions.Endpoints.Token,
                            Permissions.Endpoints.Authorization,
                            Permissions.Endpoints.Revocation,
                            Permissions.GrantTypes.Password,
                            Permissions.GrantTypes.RefreshToken,
                            //Permissions.GrantTypes.ClientCredentials,
                        }
                    },
                    cancellationToken
                );
            }
        }

        public virtual Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
