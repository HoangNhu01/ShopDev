using ShopDev.ApplicationBase;
using ShopDev.Authentication.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ShopDev.Authentication.ApplicationServices.Common
{
    public abstract class AuthenticationServiceBase : ServiceBase<AuthenticationDbContext>
    {
        protected AuthenticationServiceBase(ILogger logger, IHttpContextAccessor httpContext)
            : base(logger, httpContext) { }
    }
}
