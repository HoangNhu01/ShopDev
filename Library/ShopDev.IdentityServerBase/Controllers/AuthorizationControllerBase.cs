using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;

namespace ShopDev.IdentityServerBase.Controllers
{
    public abstract class AuthorizationControllerBase : Controller
    {
        protected readonly IOpenIddictApplicationManager _applicationManager;

        protected AuthorizationControllerBase(IOpenIddictApplicationManager applicationManager)
        {
            _applicationManager = applicationManager;
        }
    }
}
