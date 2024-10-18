using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using ShopDev.ApplicationBase.Localization;
using ShopDev.Authentication.ApplicationServices.AuthenticationModule.Abstract;
using ShopDev.Constants.Users;

namespace ShopDev.Common.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class PermissionFilterAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _permissions;
        private IPermissionService? _permissionServices;
        private LocalizationBase? _localization;
        private IHttpContextAccessor? _httpContext;

        public PermissionFilterAttribute(params string[] permissions)
        {
            _permissions = permissions;
        }

        private void GetServices(AuthorizationFilterContext context)
        {
            _permissionServices =
                context.HttpContext.RequestServices.GetRequiredService<IPermissionService>();
            _localization =
                context.HttpContext.RequestServices.GetRequiredService<LocalizationBase>();
            _httpContext =
                context.HttpContext.RequestServices.GetRequiredService<IHttpContextAccessor>();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            GetServices(context);
            var permissions = _permissionServices!.GetPermission();
            bool isGrant = false;
            var userType = _httpContext!.GetCurrentUserType();
            if (userType == UserTypes.SUPER_ADMIN)
            {
                return;
            }
            for (int i = 0; i < _permissions.Length; i++)
            {
                string? permission = _permissions[i];
                if (permissions.Contains(permission))
                {
                    isGrant = true;
                    break;
                }
            }
            if (!isGrant)
            {
                context.Result = new UnauthorizedObjectResult(
                    new { message = _localization!.Localize("error_UserNotHavePermission") }
                );
            }
        }
    }
}
