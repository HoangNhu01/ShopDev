using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ShopDev.ApplicationBase;
using ShopDev.ApplicationBase.Localization;
using ShopDev.Chat.Infrastructure.Persistence;

namespace ShopDev.Chat.ApplicationServices.Common
{
    public abstract class ChatServiceBase : ServiceBase<ChatDbContext>
    {
        protected ChatServiceBase(ILogger logger, IHttpContextAccessor httpContext)
            : base(logger, httpContext) { }

        protected ChatServiceBase(
            ILogger logger,
            IMapErrorCode mapErrorCode,
            IHttpContextAccessor httpContext,
            ChatDbContext dbContext,
            LocalizationBase localizationBase,
            IMapper mapper
        )
            : base(logger, mapErrorCode, httpContext, dbContext, localizationBase, mapper) { }
    }
}
