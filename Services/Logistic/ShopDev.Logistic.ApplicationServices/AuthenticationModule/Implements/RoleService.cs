using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ShopDev.ApplicationBase.Common;
using ShopDev.Authentication.ApplicationServices.AuthenticationModule.Abstract;
using ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.RoleDto;
using ShopDev.Authentication.ApplicationServices.Common;
using ShopDev.Authentication.Domain.Users;
using ShopDev.Constants.ErrorCodes;
using ShopDev.Constants.Role;
using ShopDev.InfrastructureBase.Exceptions;

namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Implements
{
    public class RoleService : AuthenticationServiceBase, IRoleService
    {
        public RoleService(ILogger<RoleService> logger, IHttpContextAccessor httpContext)
            : base(logger, httpContext) { }

        public RoleDto Add(CreateRolePermissionDto input)
        {
            _logger.LogInformation($"{nameof(Add)}: input = {JsonSerializer.Serialize(input)}");
            if (_dbContext.Roles.Any(s => s.Name == input.Name && !s.Deleted))
            {
                throw new UserFriendlyException(ErrorCode.RoleNameExist);
            }
            var roleInsert = _mapper.Map<Role>(input);
            _dbContext.Add(roleInsert);
            _dbContext.SaveChanges();

            foreach (var item in input.PermissionKeys.Where(i => i != null))
            {
                _dbContext.RolePermissions.Add(
                    new RolePermission { RoleId = roleInsert.Id, PermissionKey = item! }
                );
            }
            _dbContext.SaveChanges();
            return _mapper.Map<RoleDto>(roleInsert);
        }

        public void Delete(int id)
        {
            _logger.LogInformation($"{nameof(Delete)}: id = {id}");
            var role =
                _dbContext.Roles.FirstOrDefault(e => e.Id == id && !e.Deleted)
                ?? throw new UserFriendlyException(ErrorCode.RoleNotFound);
            role.Deleted = true;
            var rolePermission = _dbContext.RolePermissions.Where(e => e.RoleId == id);
            foreach (var item in rolePermission)
            {
                _dbContext.Remove(item);
            }
            _dbContext.SaveChanges();
        }

        public PagingResult<RoleDto> FindAll(FilterRoleDto input)
        {
            _logger.LogInformation($"{nameof(FindAll)}: input = {JsonSerializer.Serialize(input)}");
            var userRoleFind = (
                from user in _dbContext.Users
                join userRole in _dbContext.UserRoles on user.Id equals userRole.UserId
                where !user.Deleted && !userRole.Deleted
                select userRole
            );

            var rolePermissions =
                from role in _dbContext.Roles
                join user in _dbContext.Users on role.CreatedBy equals user.Id
                where
                    !role.Deleted
                    && !user.Deleted
                    && (input.UserType == null || role.UserType == input.UserType)
                    && (role.PermissionInWeb == input.PermissionInWeb)
                    && (input.Status == null || role.Status == input.Status)
                    && (
                        input.Keyword == null
                        || role.Name.ToLower().Contains(input.Keyword.ToLower())
                    )
                select new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    UserType = role.UserType,
                    Description = role.Description,
                    PermissionInWeb = role.PermissionInWeb,
                    Status = role.Status,
                    TotalUse = userRoleFind
                        .Where(u => u.RoleId == role.Id)
                        .Select(u => u.UserId)
                        .Distinct()
                        .Count(),
                    CreatedDate = role.CreatedDate,
                    CreatedByUserName = user.Username
                };
            var result = new PagingResult<RoleDto>();
            result.TotalItems = rolePermissions.Count();
            rolePermissions = rolePermissions.OrderByDescending(o => o.Id);
            if (input.PageSize != -1)
            {
                rolePermissions = rolePermissions.Skip(input.GetSkip()).Take(input.PageSize);
            }
            result.Items = rolePermissions;
            return result;
        }

        public RoleDto FindById(int id)
        {
            _logger.LogInformation($"{nameof(FindById)}: id = {id}");
            var role =
                _dbContext.Roles.FirstOrDefault(e => e.Id == id && !e.Deleted)
                ?? throw new UserFriendlyException(ErrorCode.RoleNotFound);

            var result = _mapper.Map<RoleDto>(role);
            var rolePermission = _dbContext.RolePermissions.Where(e => e.RoleId == id);
            result.PermissionDetails = rolePermission.ToList();
            return result;
        }

        public RoleDto Update(UpdateRolePermissionDto input)
        {
            _logger.LogInformation($"{nameof(Update)}: input = {JsonSerializer.Serialize(input)}");
            var role = _dbContext.Roles.FirstOrDefault(e => e.Id == input.Id && !e.Deleted);
            if (role == null)
            {
                throw new UserFriendlyException(ErrorCode.RoleNotFound);
            }
            if (_dbContext.Roles.Any(s => s.Id != role.Id && s.Name == input.Name && !s.Deleted))
            {
                throw new UserFriendlyException(ErrorCode.RoleNameExist);
            }
            role.Name = input.Name;
            role.Description = input.Description;

            //List permission có trong db
            var currentListPermission = _dbContext.RolePermissions.Where(e => e.RoleId == input.Id);

            //List Rolepermission bị xóa
            var removeListPermission = currentListPermission.Where(e =>
                input.PermissionKeysRemove.Contains(e.PermissionKey)
            );
            foreach (var item in removeListPermission)
            {
                _dbContext.RolePermissions.Remove(item);
            }

            foreach (var item in input.PermissionKeys)
            {
                //Thêm các rolepermission với các permission key từ input vào List permission input
                var rolePermission = _dbContext.RolePermissions.FirstOrDefault(e =>
                    e.RoleId == input.Id && e.PermissionKey == item
                );
                if (rolePermission == null)
                {
                    _dbContext.RolePermissions.Add(
                        new RolePermission
                        {
                            RoleId = input.Id,
                            PermissionKey = item ?? string.Empty,
                        }
                    );
                }
            }
            _dbContext.SaveChanges();
            return _mapper.Map<RoleDto>(role);
        }

        public void ChangeStatus(int id)
        {
            _logger.LogInformation($"{nameof(ChangeStatus)}: id = {id}");
            var role =
                _dbContext.Roles.FirstOrDefault(u => u.Id == id && !u.Deleted)
                ?? throw new UserFriendlyException(ErrorCode.RoleNotFound);

            if (role.Status == RoleStatus.ACTIVE)
            {
                role.Status = RoleStatus.DEACTIVE;
            }
            else
            {
                role.Status = RoleStatus.ACTIVE;
            }
            _dbContext.SaveChanges();
        }

        public IEnumerable<int> FindAllWebByUser()
        {
            var userId = _httpContext.GetCurrentUserId();
            var result = (
                from userRole in _dbContext.UserRoles
                join role in _dbContext.Roles on userRole.RoleId equals role.Id
                where userRole.UserId == userId && !userRole.Deleted && !role.Deleted
                select role.PermissionInWeb
            ).Distinct();
            return result;
        }

        public IEnumerable<RoleDto> FindRoleByUser(int userId)
        {
            _logger.LogInformation($"{nameof(FindRoleByUser)}: userId = {userId}");

            var result =
                from user in _dbContext.Users
                join userRole in _dbContext.UserRoles on user.Id equals userRole.UserId
                join role in _dbContext.Roles on userRole.RoleId equals role.Id
                where
                    !user.Deleted
                    && !userRole.Deleted
                    && user.Id == userId
                    && !role.Deleted
                    && role.Status == RoleStatus.ACTIVE
                select new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    UserType = role.UserType,
                    Description = role.Description,
                    PermissionInWeb = role.PermissionInWeb,
                    Status = role.Status
                };
            return result;
        }

        public IEnumerable<RoleDto> GetAll()
        {
            var userRoleFind =
                from user in _dbContext.Users
                join userRole in _dbContext.UserRoles on user.Id equals userRole.UserId
                where !user.Deleted && !userRole.Deleted
                select userRole;

            var result =
                from role in _dbContext.Roles
                where !role.Deleted && role.Status == RoleStatus.ACTIVE
                select new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    UserType = role.UserType,
                    Description = role.Description,
                    PermissionInWeb = role.PermissionInWeb,
                    Status = role.Status,
                    TotalUse = userRoleFind
                        .Where(u => u.RoleId == role.Id)
                        .Select(u => u.UserId)
                        .Distinct()
                        .Count()
                };
            return result;
        }
    }
}
