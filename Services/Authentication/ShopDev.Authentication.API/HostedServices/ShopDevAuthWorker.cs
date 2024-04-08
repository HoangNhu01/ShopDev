using ShopDev.Authentication.API.Models;
using ShopDev.Authentication.Infrastructure.Persistence;
using ShopDev.IdentityServerBase.Services;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ShopDev.Authentication.API.HostedServices
{
    public class ShopDevAuthWorker : AuthWorkerBase<AuthenticationDbContext>
    {
        private readonly IOptions<AuthWorkerConfiguration> _authWorkerConfiguration;

        public ShopDevAuthWorker(
            IServiceProvider serviceProvider,
            IOptions<AuthWorkerConfiguration> authWorkerConfiguration
        )
            : base(serviceProvider)
        {
            _authWorkerConfiguration = authWorkerConfiguration;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AuthenticationDbContext>();
            await context.Database.EnsureCreatedAsync();

            var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            if (await manager.FindByClientIdAsync("client-web", cancellationToken) is null)
            {
                var openIddictApplicationDescriptor = new OpenIddictApplicationDescriptor
                {
                    ClientId = "client-web",
                    ClientSecret = "6D283A34CBA0BC57FC07E8CEAB16C",
                    DisplayName = "Web",
                    ConsentType = ConsentTypes.Implicit,
                    Permissions =
                    {
                        Permissions.Endpoints.Token,
                        Permissions.Endpoints.Authorization,
                        Permissions.Endpoints.Revocation,
                        Permissions.GrantTypes.Password,
                        Permissions.GrantTypes.RefreshToken,
                        //Permissions.GrantTypes.ClientCredentials,
                        Permissions.GrantTypes.Password,
                        Permissions.Endpoints.Logout,
                        Permissions.GrantTypes.AuthorizationCode,
                        Permissions.ResponseTypes.Code,
                        Permissions.Scopes.Roles,
                    }
                };
                foreach (var item in _authWorkerConfiguration.Value.RedirectUris)
                {
                    openIddictApplicationDescriptor.RedirectUris.Add(new Uri(item));
                }
                foreach (var item in _authWorkerConfiguration.Value.PostLogoutRedirectUris)
                {
                    openIddictApplicationDescriptor.PostLogoutRedirectUris.Add(new Uri(item));
                }
                await manager.CreateAsync(openIddictApplicationDescriptor, cancellationToken);
            }

            if (await manager.FindByClientIdAsync("client-app", cancellationToken) is null)
            {
                await manager.CreateAsync(
                    new OpenIddictApplicationDescriptor
                    {
                        ClientId = "client-app",
                        ClientSecret = "2E72EDF5F596BC30248178FA022F9",
                        DisplayName = "App",
                        Permissions =
                        {
                            Permissions.Endpoints.Token,
                            Permissions.Endpoints.Authorization,
                            Permissions.Endpoints.Revocation,
                            IdentityServerBase.Constants.GrantTypes.App,
                            Permissions.GrantTypes.Password,
                            Permissions.GrantTypes.RefreshToken,
                            //Permissions.GrantTypes.ClientCredentials,
                        }
                    },
                    cancellationToken
                );
            }
        }
    }
}
