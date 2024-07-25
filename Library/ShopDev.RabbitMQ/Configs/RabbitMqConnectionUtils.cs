using System.Net.Security;
using RabbitMQ.Client;

namespace ShopDev.RabbitMQ.Configs
{
    public static class RabbitMqConnectionUtils
    {
        /// <summary>
        /// Tạo connection tới rabbitmq
        /// </summary>
        /// <param name="config"></param>
        /// <param name="dispatchConsumersAsync"></param>
        /// <returns></returns>
        public static IConnection CreateConnection(
            this RabbitMqConfig config,
            bool dispatchConsumersAsync = false
        )
        {
            //Config connection to Rabbit
            ConnectionFactory connection =
                new()
                {
                    UserName = config.Username,
                    Password = config.Password,
                    HostName = config.HostName,
                    Port = config.Port
                };
            //VH to define app service
            if (!string.IsNullOrWhiteSpace(config.VirtualHost))
            {
                connection.VirtualHost = config.VirtualHost;
            }
            //Config SSL
            if (!string.IsNullOrWhiteSpace(config.Ssl?.CertPath))
            {
                //connection.Ssl.ServerName = config.Ssl!.ServerName;
                connection.Ssl.Enabled = true;
                connection.Ssl.CertPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    config.Ssl!.CertPath!
                );
                connection.Ssl.AcceptablePolicyErrors =
                    SslPolicyErrors.RemoteCertificateNameMismatch
                    | SslPolicyErrors.RemoteCertificateChainErrors;
            }

            //connection.DispatchConsumersAsync = dispatchConsumersAsync;
            var channel = connection.CreateConnection();
            return channel;
        }
    }
}
