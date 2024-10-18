using MB.SignalR.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using ShopDev.Constants.Users;

namespace ShopDev.SignalR
{
    [Authorize]
    public abstract class BaseHub<TClient> : Hub<TClient>
        where TClient : class, IHubClient
	{
        private readonly ILogger<BaseHub<TClient>> _logger;

        public BaseHub(ILogger<BaseHub<TClient>> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.GetClaim(ClaimTypes.UserId);
            _logger.LogInformation($"{nameof(OnConnectedAsync)}: userId = {userId} has connected");
            await JoinGroup(GroupNames.NOTIFY_GROUP + userId);
			await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.GetClaim(ClaimTypes.UserId);
            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                GroupNames.NOTIFY_GROUP + userId
            );
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Thêm user vào một group
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public abstract Task SendMessage(object message);
    }
}
