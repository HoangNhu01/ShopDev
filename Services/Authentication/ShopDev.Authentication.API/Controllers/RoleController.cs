using System.Net;
using ShopDev.Authentication.ApplicationServices.AuthenticationModule.Abstract;
using ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.RoleDto;
using ShopDev.Authentication.Domain.Users;
using ShopDev.Common.Filters;
using ShopDev.Constants.RolePermission.Constant;
using ShopDev.UserRolePermission;
using ShopDev.Utils.Net.Request;
using ShopDev.WebAPIBase.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShopDev.Authentication.API.Controllers
{
    [Authorize]
    [Route("api/auth/role")]
    [AuthorizeAdminUserTypeFilter]
    [ApiController]
    public class RoleController : ApiControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(ILogger<RoleController> logger, IRoleService roleService)
            : base(logger)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Thêm Role
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [ProducesResponseType(typeof(ApiResponse<RoleDto>), (int)HttpStatusCode.OK)]
        [PermissionFilter(PermissionKeys.UserButtonPermissionAdd)]
        public ApiResponse Add(CreateRolePermissionDto input)
        {
            try
            {
                return new(_roleService.Add(input));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// update Role
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("update")]
        [ProducesResponseType(typeof(ApiResponse<RoleDto>), (int)HttpStatusCode.OK)]
        [PermissionFilter(PermissionKeys.UserButtonRolePermissionUpdate)]
        public ApiResponse Update(UpdateRolePermissionDto input)
        {
            try
            {
                return new(_roleService.Update(input));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Xóa Role
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(typeof(ApiResponse<RoleDto>), (int)HttpStatusCode.OK)]
        [PermissionFilter(PermissionKeys.UserButtonRolePermissionDelete)]
        public ApiResponse Delete(int id)
        {
            try
            {
                _roleService.Delete(id);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Find Role
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("find-by-id/{id}")]
        [ProducesResponseType(typeof(ApiResponse<RoleDto>), (int)HttpStatusCode.OK)]
        [PermissionFilter(PermissionKeys.UserButtonRolePermissionUpdate)]
        public ApiResponse FindById(int id)
        {
            try
            {
                return new(_roleService.FindById(id));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Xem danh sách Role
        /// </summary>
        /// <returns></returns>
        [HttpGet("find-all")]
        [ProducesResponseType(typeof(ApiResponse<RoleDto>), (int)HttpStatusCode.OK)]
        [PermissionFilter(
            PermissionKeys.UserButtonPermissionSetting,
            PermissionKeys.UserTableRolePermission
        )]
        public ApiResponse FindAll([FromQuery] FilterRoleDto input)
        {
            try
            {
                return new(_roleService.FindAll(input));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Khoá role
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("change-status/{id}")]
        [ProducesResponseType(typeof(ApiResponse<Role>), (int)HttpStatusCode.OK)]
        [PermissionFilter(PermissionKeys.UserButtonRolePermissionLock)]
        public ApiResponse ChangeStatus(int id)
        {
            try
            {
                _roleService.ChangeStatus(id);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Lấy danh sách web user có quyền
        /// </summary>
        /// <returns></returns>
        [HttpGet("find-all-web-user")]
        [ProducesResponseType(typeof(ApiResponse<User>), (int)HttpStatusCode.OK)]
        public ApiResponse FindAllWebByUser()
        {
            try
            {
                return new(_roleService.FindAllWebByUser());
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Tìm role theo user
        /// </summary>
        /// <returns></returns>
        [HttpGet("find-by-user/{userId}")]
        [ProducesResponseType(typeof(ApiResponse<RoleDto>), (int)HttpStatusCode.OK)]
        [PermissionFilter(PermissionKeys.UserButtonAccountManagerUpdatePermission)]
        public ApiResponse FindRoleByUser(int userId)
        {
            try
            {
                return new(_roleService.FindRoleByUser(userId));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Danh sách tất cả role không chia quyền theo web
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-all")]
        [ProducesResponseType(typeof(ApiResponse<RoleDto>), (int)HttpStatusCode.OK)]
        [PermissionFilter(PermissionKeys.UserButtonAccountManagerUpdatePermission)]
        public ApiResponse GetAll()
        {
            try
            {
                return new(_roleService.GetAll());
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}
