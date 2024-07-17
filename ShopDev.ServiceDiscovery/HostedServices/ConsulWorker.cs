using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ShopDev.ServiceDiscovery.Config;

namespace ShopDev.ServiceDiscovery.HostedServices
{
    public class ConsulWorker : IHostedService
    {
        private readonly IConsulClient _consulClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ConsulWorker> _logger;
        private readonly ConsulConfig _config;

        public ConsulWorker(
            IConsulClient consulClient,
            IConfiguration configuration,
            ILogger<ConsulWorker> logger,
            IOptions<ConsulConfig> config
        )
        {
            _consulClient = consulClient;
            _configuration = configuration;
            _logger = logger;
            _config = config.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var registration = new AgentServiceRegistration
            {
                ID = _config.ServiceId,
                Name = _config.ServiceName,
                Address = _config.ServiceHost,
                Port = _config.ServicePort,
                Tags = [_config.ServiceName]
            };

            //var check = new AgentServiceCheck
            //{
            //    HTTP = _config.HealthCheckUrl,
            //    Interval = TimeSpan.FromSeconds(_config.HealthCheckIntervalSeconds),
            //    Timeout = TimeSpan.FromSeconds(_config.HealthCheckTimeoutSeconds)
            //};

            //registration.Checks = [check];

            _logger.LogInformation($"Registering service with Consul: {registration.Name}");

            await _consulClient.Agent.ServiceDeregister(registration.ID, cancellationToken);
            await _consulClient.Agent.ServiceRegister(registration, cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            AgentServiceRegistration registration = new() { ID = _config.ServiceId };

            _logger.LogInformation($"Deregistering service from Consul: {registration.ID}");

            await _consulClient.Agent.ServiceDeregister(registration.ID, cancellationToken);
        }
    }
}
