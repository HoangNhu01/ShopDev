using ShopDev.SignalR;

namespace ShopDev.Chat.API.Hubs
{
	public interface INotificationHub : IHubClient
	{
		Task SendMessage(object message);
	}
}
