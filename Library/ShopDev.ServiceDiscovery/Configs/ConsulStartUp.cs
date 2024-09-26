using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShopDev.ServiceDiscovery.Config;
using ShopDev.ServiceDiscovery.HostedServices;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Consul;

namespace ShopDev.ServiceDiscovery.Configs
{
    public static class ConsulStartUp
    {
        public static void ServiceDiscovery(this WebApplicationBuilder builder)
        {
            ConfigurationManager configurationManager = builder.Configuration;
            builder.Services.AddHttpClient().AddServiceDiscovery(x => x.UseConsul());

            builder.Services.AddSingleton<IConsulClient, ConsulClient>(_ => new ConsulClient(
                consulConfig =>
                {
                    consulConfig.Address = new($"{configurationManager["Consul:ConsulAddress"]}");
                }
            ));
            builder.Services.AddSingleton<IHostedService, ConsulWorker>();
        }
    }
}
