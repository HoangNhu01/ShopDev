using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShopDev.ServiceDiscovery.Config;
using ShopDev.ServiceDiscovery.HostedServices;

namespace ShopDev.ServiceDiscovery.Configs
{
    public static class ConsulStartUp
    {
        public static void ServiceDiscovery(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IConsulClient, ConsulClient>(_ => new ConsulClient(
                consulConfig =>
                {
                    consulConfig.Address = new Uri("http://localhost:8500");
                }
            ));
            builder.Services.AddSingleton<IHostedService, ConsulWorker>();
            builder.Services.Configure<ConsulConfig>(builder.Configuration.GetSection("Consul"));
        }
    }
}
