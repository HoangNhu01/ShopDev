using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ShopDev.ApplicationBase;
using ShopDev.ApplicationBase.Localization;
using ShopDev.Authentication.Infrastructure.Persistence;

namespace ShopDev.Authentication.ApplicationServices.Common
{
    public abstract class AuthenticationServiceBase : ServiceBase<AuthenticationDbContext>
    {
        protected AuthenticationServiceBase(ILogger logger, IHttpContextAccessor httpContext)
            : base(logger, httpContext) { }

        protected AuthenticationServiceBase(
            ILogger logger,
            IMapErrorCode mapErrorCode,
            IHttpContextAccessor httpContext,
            AuthenticationDbContext dbContext,
            LocalizationBase localizationBase,
            IMapper mapper
        )
            : base(logger, mapErrorCode, httpContext, dbContext, localizationBase, mapper) { }
    }
}
