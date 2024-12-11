using System.Runtime.CompilerServices;

namespace ShopDev.Kafka.Interfaces
{
	public interface IProducerService
	{
		Task PublishMessageAsync<TMessage>(
			TMessage entity,
			[CallerMemberName] string? callerMethodName = null
		)
			where TMessage : class;
	}
}
