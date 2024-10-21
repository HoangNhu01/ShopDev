using AutoMapper;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ShopDev.ApplicationBase.Localization;
using ShopDev.Chat.ApplicationServices.ChatModule.Abstract;
using ShopDev.Chat.ApplicationServices.ChatModule.Dtos;
using ShopDev.Chat.ApplicationServices.Common;
using ShopDev.Chat.Infrastructure.Persistence;
using ShopDev.Chat.Infrastructure.Repositories.Abstracts;
using ShopDev.InfrastructureBase.Exceptions;

namespace ShopDev.Chat.Domain.Chats.ChatModule.Implements
{
    public class ConversationService : ChatServiceBase, IConversationService
    {
        private readonly IConversationRepository _conversationRepository;

        public ConversationService(
            ILogger<ConversationService> logger,
            IMapErrorCode mapErrorCode,
            IHttpContextAccessor httpContext,
            ChatDbContext dbContext,
            LocalizationBase localizationBase,
            IMapper mapper,
            IConversationRepository conversationRepository
        )
            : base(logger, mapErrorCode, httpContext, dbContext, localizationBase, mapper)
        {
            _conversationRepository = conversationRepository;
        }

        public async Task<ConversationDto> Create(List<string> userIds)
        {
            var newConversation = await _conversationRepository.GetOrCreateChatAsync(userIds);
            return _mapper.Map<ConversationDto>(newConversation);
        }

        public async Task<ConversationDto> FindById(string id)
        {
            int userId = _httpContext.GetCurrentUserId();
            var conversation =
                await _conversationRepository.GetFirstOrDefaultAsync(userId: userId.ToString(), id)
                ?? throw new UserFriendlyException(1000);

            return _mapper.Map<ConversationDto>(conversation);
        }

        public async Task<PagingResult<ConversationDto>> FindAll(PagingConversationDto input)
        {
            int userId = _httpContext.GetCurrentUserId();
            var conversation =
                await _conversationRepository.GetPagingAsync(
                    userId: userId.ToString(),
                    new() { OffSet = input.GetSkip(), Limit = input.PageSize }
                ) ?? throw new UserFriendlyException(1000);

            return new()
            {
                Items = _mapper.Map<IEnumerable<ConversationDto>>(conversation.Items),
                TotalItems = conversation.TotalItems
            };
        }
    }
}
