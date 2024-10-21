using System;
using Consul;
using MB.SignalR.Constants;
using OpenIddict.Abstractions;
using ShopDev.Chat.ApplicationServices.ChatModule.Abstract;
using ShopDev.Chat.ApplicationServices.ChatModule.Dtos;
using ShopDev.Constants.Users;
using ShopDev.SignalR;
using StackExchange.Redis;

namespace ShopDev.Chat.API.Hubs
{
    public class ChatHub : BaseHub<IChatHub>
    {
        private readonly IMessageService _messageService;
        private readonly ILogger<ChatHub> _logger;
		private readonly MessageBrokerService _messageBroker;

		public ChatHub(ILogger<ChatHub> logger, IMessageService messageService, MessageBrokerService messageBroker)
            : base(logger)
        {
            _messageService = messageService;
            _logger = logger;
			_messageBroker = messageBroker;
		}

		public async Task SendMessage(MessageCreateDto message)
        {
            var newMessage = await _messageService.Create(message);
            await Clients
                .Group(GroupNames.NOTIFY_GROUP + newMessage.ConversationId)
                .SendMessage(newMessage);
			_messageBroker.SendMessage(message);
		}

		public async Task LeaveGroup(string conversationId)
        {
            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                GroupNames.NOTIFY_GROUP + conversationId
            );
            //await base.OnDisconnectedAsync(null);
        }
    }
}
