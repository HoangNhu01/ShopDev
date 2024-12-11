namespace ShopDev.Kafka.Configs
{
    public class KafkaConfig
    {
        public required string HostName { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public int Port { get; set; }
        public KafkaSsl? Ssl { get; set; }
    }

    public class KafkaSsl
    {
        public required string ServerName { get; set; }
        public required string CertPath { get; set; }
    }
}
