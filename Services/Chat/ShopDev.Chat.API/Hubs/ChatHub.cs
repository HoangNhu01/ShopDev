using Consul;
using ShopDev.SignalR;

namespace ShopDev.Chat.API.Hubs
{
	public class ChatHub : BaseHub<IChatHub>
	{

		public ChatHub(ILogger<ChatHub> logger) : base(logger)
		{
		}

		public override async Task SendMessage(object message)
		{
			await Clients.All.NotifyUserConnected(message.ToString()!);
		}
	}
}
