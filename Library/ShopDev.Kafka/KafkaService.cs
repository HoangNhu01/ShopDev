using Microsoft.Extensions.Options;
using ShopDev.Kafka.Configs;

namespace ShopDev.Kafka
{
	public abstract class KafkaService
	{
		private readonly KafkaConfig _config;

		protected KafkaService(IOptions<KafkaConfig> options)
		{
			_config = options.Value;
		}

	}
}
