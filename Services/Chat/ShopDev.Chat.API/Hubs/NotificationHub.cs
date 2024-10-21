using System.Text.Json;
using DocumentFormat.OpenXml.Wordprocessing;
using MB.SignalR.Constants;
using Microsoft.AspNetCore.SignalR;
using ShopDev.Chat.ApplicationServices.ChatModule.Abstract;
using ShopDev.Chat.ApplicationServices.ChatModule.Dtos;
using ShopDev.SignalR;

namespace ShopDev.Chat.API.Hubs
{
    public class NotificationHub : BaseHub<INotificationHub>
    {
        private readonly IMessageService _messageService;
        private readonly ILogger<NotificationHub> _logger;
        private readonly MessageBrokerService _messageBroker;

        public NotificationHub(
            ILogger<NotificationHub> logger,
            IMessageService messageService,
            MessageBrokerService messageBroker
        )
            : base(logger)
        {
            _messageService = messageService;
            _logger = logger;
            _messageBroker = messageBroker;
            // Nhận thông điệp từ RabbitMQ và gửi đến client khi khởi tạo Hub
            _messageBroker.ReceiveMessage(
                async (message) =>
                {
                    var messageDeserialize = JsonSerializer.Deserialize<MessageDto>(message);
                    await Clients
                        .Group(messageDeserialize!.ConversationName)
                        .SendMessage(messageDeserialize);
                }
            );
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
