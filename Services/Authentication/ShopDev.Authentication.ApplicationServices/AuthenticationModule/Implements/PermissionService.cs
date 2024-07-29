using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShopDev.Authentication.ApplicationServices.AuthenticationModule.Abstract;
using ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.PermissionDto;
using ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.PermissionDtos;
using ShopDev.Authentication.ApplicationServices.Common;
using ShopDev.Authentication.Domain.Users;
using ShopDev.Constants.Domain.Auth.Role;
using ShopDev.Constants.ErrorCodes;
using ShopDev.Constants.RolePermission;
using ShopDev.Constants.Users;
using ShopDev.InfrastructureBase.Exceptions;

namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Implements
{
    public class PermissionService : AuthenticationServiceBase, IPermissionService
    {
        public PermissionService(
            ILogger<PermissionService> logger,
            IHttpContextAccessor httpContext
        )
            : base(logger, httpContext) { }

        public List<string> GetPermission()
        {
            int userId = _httpContext.GetCurrentUserId();
            int userType = _httpContext.GetCurrentUserType();
            _logger.LogInformation(
                $"{nameof(GetPermission)}: userId = {userId}, userType = {userType}"
            );
            return
            [
                .. _dbContext
                    .UserRoles.Include(x => x.Role)
                    .ThenInclude(x => x.RolePermissions)
                    .Where(x =>
                       x.UserId == userId
                    )
                    .SelectMany(x => x.Role.RolePermissions.Select(s => s.PermissionKey)).Distinct()
            ];
        }

        public List<string> GetPermissionInternalService(int? permissionInWeb, int userId)
        {
            _logger.LogInformation(
                $"{nameof(GetPermissionInternalService)}: permissionInWeb = {permissionInWeb}, userId = {userId}"
            );
            var user =
                _dbContext.Users.Find(userId)
                ?? throw new UserFriendlyException(ErrorCode.UserNotFound);
            return GetPermissionByUser(
                new()
                {
                    UserId = userId,
                    UserType = user.UserType,
                    PermissionInWeb = permissionInWeb
                }
            );
        }

        /// <summary>
        /// Lấy danh sách các quyền theo user
        /// </summary>
        /// <returns></returns>
        private List<string> GetPermissionByUser(GetUserPermissionDto userPermission)
        {
            if (userPermission.UserType == UserTypes.SUPER_ADMIN)
            {
                if (
                    userPermission.PermissionInWeb.HasValue
                    || userPermission.PermissionInWeb == PermissionInWebs.Core
                )
                {
                    return [.. PermissionConfig.CoreConfigs.Select(o => o.Key)];
                }
            }

            return
            [
                .. _dbContext
                    .UserRoles.Include(x => x.Role)
                    .ThenInclude(x => x.RolePermissions)
                    .Where(x =>
                        (
                            !userPermission.PermissionInWeb.HasValue
                            || x.Role.PermissionInWeb == userPermission.PermissionInWeb.Value
                        )
                        && x.UserId == userPermission.UserId
                    )
                    .SelectMany(x => x.Role.RolePermissions.Select(s => s.PermissionKey))
            ];
        }

        public List<string> GetPermissionInWeb(int? permissionInWeb)
        {
            var userId = _httpContext.GetCurrentUserId();
            var userType = _httpContext.GetCurrentUserType();
            _logger.LogInformation(
                $"{nameof(GetPermissionInWeb)}: userId = {userId}, userType = {userType}, permissionInWeb = {permissionInWeb}"
            );
            return GetPermissionByUser(
                new()
                {
                    UserId = userId,
                    UserType = userType,
                    PermissionInWeb = permissionInWeb
                }
            );
        }

        public IEnumerable<PermissionDetailDto> FindAllPermission(int permissionConfig)
        {
            _logger.LogInformation($"{nameof(FindAllPermission)}");
            return permissionConfig switch
            {
                PermissionInWebs.User
                    => PermissionConfig.UserConfigs.Select(x => new PermissionDetailDto
                    {
                        Key = x.Key,
                        ParentKey = x.Value.ParentKey,
                        Label = L(x.Value.LName),
                        Icon = x.Value.Icon
                    }),
                PermissionInWebs.Core
                    => PermissionConfig.CoreConfigs.Select(x => new PermissionDetailDto
                    {
                        Key = x.Key,
                        ParentKey = x.Value.ParentKey,
                        Label = L(x.Value.LName),
                        Icon = x.Value.Icon
                    }),
                _ => [],
            };
        }

        public IEnumerable<PermissionInWebDto> FindByPermissionInWeb()
        {
            _logger.LogInformation($"{nameof(FindByPermissionInWeb)}");
            return _dbContext
                .Roles.Include(x => x.UserRoles)
                .GroupBy(r => r.PermissionInWeb)
                .Select(x => new PermissionInWebDto
                {
                    PermissionInWeb = x.Key,
                    TotalRole = x.Select(o => o.Id).Count(),
                    TotalUser = x.SelectMany(s => s.UserRoles).Count(),
                });
        }
    }
}
