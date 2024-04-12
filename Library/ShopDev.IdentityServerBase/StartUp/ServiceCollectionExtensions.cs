using ShopDev.IdentityServerBase.Constants;
using ShopDev.IdentityServerBase.Services;
using ShopDev.Utils.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ShopDev.IdentityServerBase.StartUp
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCommonIdentityServer<TDbContext, TAuthWorker>(
            this IServiceCollection services,
            ConfigurationManager configuration
        )
            where TDbContext : DbContext
            where TAuthWorker : AuthWorkerBase<TDbContext>
        {
            services
                .AddOpenIddict()
                // Register the OpenIddict core components.
                .AddCore(options =>
                {
                    // Configure OpenIddict to use the Entity Framework Core stores and models.
                    // Note: call ReplaceDefaultEntities() to replace the default entities.
                    options.UseEntityFrameworkCore().UseDbContext<TDbContext>();
                })
                // Register the OpenIddict server components.
                .AddServer(options =>
                {
                    // Enable the token endpoint.
                    options
                        .SetAuthorizationEndpointUris("/connect/authorize")
                        .SetTokenEndpointUris("/connect/token")
                        .SetUserinfoEndpointUris("/connect/userinfo")
                        .SetLogoutEndpointUris("/connect/logout")
                        .SetIntrospectionEndpointUris("/.well-known/openid-configuration");

                    // Enable the client credentials flow.
                    options
                        .AllowPasswordFlow()
                        .AllowRefreshTokenFlow()
                        .AllowAuthorizationCodeFlow()
                        .AllowCustomFlow(GrantTypes.App); // Add your custom grant type here.
                    options.SetAccessTokenLifetime(TimeSpan.FromHours(2));
                    options.SetRefreshTokenLifetime(TimeSpan.FromDays(3));

                    // Accept anonymous clients (i.e clients that don't send a client_id).
                    // options.AcceptAnonymousClients();

                    // Disable mã hóa token (Vd: dùng dòng này thì jwt.io đọc được, cmt vào thì ko đọc đc)
                    options.DisableAccessTokenEncryption();

                    // Register the signing and encryption credentials.
                    var rsaSecurityKey = CryptographyUtils.ReadKey(
                        configuration.GetValue<string>("IdentityServer:PublicKey")!,
                        configuration.GetValue<string>("IdentityServer:PrivateKey")!
                    );
                    options.AddEncryptionKey(rsaSecurityKey);
                    options.AddSigningKey(rsaSecurityKey);

                    // Register the ASP.NET Core host and configure the ASP.NET Core options.
                    options
                        .UseAspNetCore()
                        .EnableTokenEndpointPassthrough()
                        .EnableAuthorizationEndpointPassthrough()
                        .EnableLogoutEndpointPassthrough()
                        .DisableTransportSecurityRequirement(); //không bắt https
                })
                // Register the OpenIddict validation components.
                .AddValidation(options =>
                {
                    options.EnableTokenEntryValidation();
                    options.EnableAuthorizationEntryValidation();
                    // Import the configuration from the local OpenIddict server instance.
                    options.UseLocalServer();

                    // Register the ASP.NET Core host.
                    options.UseAspNetCore();
                });

            // Register the worker responsible of seeding the database with the sample clients.
            // Note: in a real world application, this step should be part of a setup script.
            services.AddHostedService<TAuthWorker>();
        }
    }
}
