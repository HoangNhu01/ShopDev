using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ShopDev.ApplicationBase.Common;
using ShopDev.ApplicationBase.Localization;
using ShopDev.Chat.ApplicationServices.ChatModule.Abstract;
using ShopDev.Chat.ApplicationServices.ChatModule.Dtos;
using ShopDev.Chat.ApplicationServices.Common;
using ShopDev.Chat.Infrastructure.Persistence;
using ShopDev.Chat.Infrastructure.Repositories.Abstracts;
using ShopDev.InfrastructureBase.Exceptions;

namespace ShopDev.Chat.Domain.Chats.ChatModule.Implements
{
    public class MessageService : ChatServiceBase, IMessageService
    {
        private readonly IMessageRepository _messageRepository;

        public MessageService(
            ILogger<MessageService> logger,
            IMapErrorCode mapErrorCode,
            IHttpContextAccessor httpContext,
            ChatDbContext dbContext,
            LocalizationBase localizationBase,
            IMapper mapper,
            IMessageRepository messageRepository
        )
            : base(logger, mapErrorCode, httpContext, dbContext, localizationBase, mapper)
        {
            _messageRepository = messageRepository;
        }

        public async Task<MessageDto> Create(MessageCreateDto input)
        {
            var message = await _messageRepository.CreateMessageAsync(_mapper.Map<Message>(input));
            return _mapper.Map<MessageDto>(message);
        }

        public async Task<MessageDto> FindById(string id)
        {
            var message =
                await _messageRepository.GetFirstOrDefaultAsync(id)
                ?? throw new UserFriendlyException(1000);
            return _mapper.Map<MessageDto>(message);
        }

        public async Task<PagingResult<MessageDto>> FindById(
            string chatId,
            PagingRequestBaseLimitOffset input
        )
        {
            var result = await _messageRepository.GetPagingAsync(chatId, input);
            return new()
            {
                Items = _mapper.Map<IEnumerable<MessageDto>>(result.Items),
                TotalItems = result.TotalItems
            };
        }
    }
}
