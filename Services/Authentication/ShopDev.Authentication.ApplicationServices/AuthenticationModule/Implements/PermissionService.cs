using ShopDev.Authentication.ApplicationServices.AuthenticationModule.Abstract;
using ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.PermissionDto;
using ShopDev.Authentication.ApplicationServices.Common;
using ShopDev.Constants.ErrorCodes;
using ShopDev.Constants.Role;
using ShopDev.Constants.RolePermission;
using ShopDev.Constants.Users;
using ShopDev.InfrastructureBase.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

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
            var userId = _httpContext.GetCurrentUserId();
            var userType = _httpContext.GetCurrentUserType();
            _logger.LogInformation(
                $"{nameof(GetPermission)}: userId = {userId}, userType = {userType}"
            );

            var query =
                from userRole in _dbContext.UserRoles
                join role in _dbContext.Roles on userRole.RoleId equals role.Id
                join rolePermission in _dbContext.RolePermissions
                    on role.Id equals rolePermission.RoleId
                where userRole.UserId == userId && !role.Deleted && !userRole.Deleted
                select rolePermission.PermissionKey;

            return [.. query.Distinct()];
        }

        public IEnumerable<string> GetPermissionInternalService(int? permissionInWeb, int userId)
        {
            _logger.LogInformation(
                $"{nameof(GetPermissionInternalService)}: permissionInWeb = {permissionInWeb}, userId = {userId}"
            );
            var user =
                _dbContext
                    .Users.Select(x => new { x.Id, x.UserType })
                    .FirstOrDefault(x => x.Id == userId)
                ?? throw new UserFriendlyException(ErrorCode.UserNotFound);
            return GetPermissionByUser(userId, user.UserType, permissionInWeb);
        }

        /// <summary>
        /// Lấy danh sách các quyền theo user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userType"></param>
        /// <param name="permissionInWeb"></param>
        /// <returns></returns>
        private List<string> GetPermissionByUser(int userId, int userType, int? permissionInWeb)
        {
            var result = new List<string>();
            if (userType == UserTypes.SUPER_ADMIN)
            {
                if (permissionInWeb == null || permissionInWeb == PermissionInWebs.Core)
                {
                    result.AddRange(PermissionConfig.CoreConfigs.Select(o => o.Key));
                }

                if (permissionInWeb == null || permissionInWeb == PermissionInWebs.User)
                {
                    result.AddRange(PermissionConfig.UserConfigs.Select(o => o.Key));
                }
            }
            else
            {
                result = (
                    from userRole in _dbContext.UserRoles
                    join role in _dbContext.Roles on userRole.RoleId equals role.Id
                    join rolePermission in _dbContext.RolePermissions
                        on role.Id equals rolePermission.RoleId
                    where
                        (permissionInWeb == null || role.PermissionInWeb == permissionInWeb)
                        && userRole.UserId == userId
                        && !role.Deleted
                        && !userRole.Deleted
                    select rolePermission.PermissionKey
                ).ToList();
            }
            return result;
        }

        public IEnumerable<string> GetPermissionInWeb(int? permissionInWeb)
        {
            var userId = _httpContext.GetCurrentUserId();
            var userType = _httpContext.GetCurrentUserType();
            _logger.LogInformation(
                $"{nameof(GetPermissionInWeb)}: userId = {userId}, userType = {userType}, permissionInWeb = {permissionInWeb}"
            );
            return GetPermissionByUser(userId, userType, permissionInWeb);
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
                _ => new List<PermissionDetailDto>(),
            };
        }

        public IEnumerable<PermissionInWebDto> FindByPermissionInWeb()
        {
            var query = _dbContext
                .Roles.Where(r => !r.Deleted)
                .GroupBy(r => r.PermissionInWeb)
                .Select(roles => new PermissionInWebDto
                {
                    PermissionInWeb = roles.Key,
                    TotalRole = roles.Select(o => o.Id).Count(),
                    TotalUser = (
                        from role in roles
                        join userRole in _dbContext.UserRoles on role.Id equals userRole.RoleId
                        join user in _dbContext.Users on userRole.UserId equals user.Id
                        where !userRole.Deleted && !user.Deleted
                        select userRole.UserId
                    ).Count(),
                });
            return query;
        }
    }
}
