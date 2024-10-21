using ShopDev.Chat.ApplicationServices.ChatModule.Dtos;
using ShopDev.SignalR;

namespace ShopDev.Chat.API.Hubs
{
	public interface IChatHub : IHubClient
	{
		Task SendMessage(object message);
	}
}
