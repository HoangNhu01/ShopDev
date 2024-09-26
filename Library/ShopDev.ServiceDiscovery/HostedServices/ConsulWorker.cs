using System.Net;
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
        private Timer _timer;

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
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(9));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var registration = new AgentServiceRegistration
                {
                    ID = $"{_config.ServiceId}_{Dns.GetHostName()}",
                    Name = _config.ServiceName,
                    Address = Dns.GetHostName(),
                    Port = _config.ServicePort,
                    Tags = [_config.ServiceName],
                    Check = new AgentServiceCheck()
                    {
                        TTL = TimeSpan.FromSeconds(10), // TTL cho health check
                        DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1) // Thời gian sau khi hủy đăng ký nếu dịch vụ không gửi heartbeat
                    }
                };

                _logger.LogInformation(
                    $"Registering service with Consul : {registration.Name}, HostName: {Dns.GetHostName()}"
                );

                await _consulClient.Agent.ServiceDeregister(registration.ID, cancellationToken);
                await _consulClient.Agent.ServiceRegister(registration, cancellationToken);
            }
            catch
            {
                await Task.CompletedTask;
            }
        }

        private void DoWork(object? state)
        {
            _logger.LogInformation($"TTL check with Consul: {_config.ServiceName}");
            _consulClient
                .Agent.PassTTL($"service:{Dns.GetHostName()}", "TTL passed")
                .GetAwaiter()
                .GetResult();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            AgentServiceRegistration registration = new() { ID = _config.ServiceId };

            _logger.LogInformation($"Deregistering service from Consul: {registration.ID}");

            await _consulClient.Agent.ServiceDeregister(registration.ID, cancellationToken);
        }
    }
}
