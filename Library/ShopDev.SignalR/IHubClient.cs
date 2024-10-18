namespace ShopDev.SignalR
{
    public interface IHubClient
	{
        Task ReceiveMessage(string message);
		Task NotifyUserConnected(string message);
		Task NotifyUserDisconnected(string message);
	}
}
