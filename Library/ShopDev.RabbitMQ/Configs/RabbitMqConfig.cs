namespace ShopDev.RabbitMQ.Configs
{
    public class RabbitMqConfig
    {
        public required string HostName { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public int Port { get; set; }
        public string? VirtualHost { get; set; }
        public RabbitMqSsl? Ssl { get; set; }
    }

    public class RabbitMqSsl
    {
        public required string ServerName { get; set; }
        public required string CertPath { get; set; }
    }
}
