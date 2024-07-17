using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopDev.Authentication.ApplicationServices.AuthenticationModule.Abstract;
using ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.RoleDto;
using ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.UserDto;
using ShopDev.Authentication.Domain.Users;
using ShopDev.Common.Filters;
using ShopDev.Constants.RolePermission.Constant;
using ShopDev.UserRolePermission;
using ShopDev.Utils.Net.Request;
using ShopDev.WebAPIBase.Controllers;

namespace ShopDev.Authentication.API.Controllers
{
    //[Authorize]
    //[AuthorizeAdminUserTypeFilter]
    [Route("api/auth/user")]
    [ApiController]
    public class UsersController : ApiControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(ILogger<UsersController> logger, IUserService userService)
            : base(logger)
        {
            _userService = userService;
        }

        /// <summary>
        /// Xem danh sách User CMS (ADMIN)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("find-all")]
        [ProducesResponseType(typeof(ApiResponse<User>), (int)HttpStatusCode.OK)]
        [PermissionFilter(PermissionKeys.UserTableAccountManager)]
        public ApiResponse FindAll([FromQuery] FilterUserDto input)
        {
            try
            {
                return new(_userService.FindAll(input));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Tìm kiếm người dùng theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<User>), (int)HttpStatusCode.OK)]
        [PermissionFilter(PermissionKeys.UserTableAccountManager)]
        public ApiResponse FindById(int id)
        {
            try
            {
                return new(_userService.GetById(id));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Thông tin tài khoản từ userId token
        /// </summary>
        /// <returns></returns>
        [HttpGet("find-by-user")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), (int)HttpStatusCode.OK)]
        public ApiResponse FindByUser()
        {
            try
            {
                return new(_userService.FindByUser());
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Thêm User
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [ProducesResponseType(typeof(ApiResponse<User>), (int)HttpStatusCode.OK)]
        //[PermissionFilter(PermissionKeys.UserButtonAccountManagerAdd)]
        public async Task<ApiResponse> Add([FromBody] CreateUserDto input)
        {
            try
            {
                await _userService.Create(input);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Cập nhật User
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("update")]
        [ProducesResponseType(typeof(ApiResponse<User>), (int)HttpStatusCode.OK)]
        //[PermissionFilter(PermissionKeys.UserUpdate)]
        public ApiResponse Update([FromBody] UpdateUserDto input)
        {
            try
            {
                _userService.Update(input);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Xóa User
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("delete/{id}")]
        [ProducesResponseType(typeof(ApiResponse<User>), (int)HttpStatusCode.OK)]
        [PermissionFilter(PermissionKeys.UserButtonAccountManagerDelete)]
        public ApiResponse Delete(int id)
        {
            try
            {
                _userService.Delete(id);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Thay đổi trạng thái tai khoản
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("change-status/{id}")]
        [ProducesResponseType(typeof(ApiResponse<User>), (int)HttpStatusCode.OK)]
        [PermissionFilter(PermissionKeys.UserButtonAccountManagerLock)]
        public ApiResponse ChangeStatus(int id)
        {
            try
            {
                _userService.ChangeStatus(id);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Tự thay đổi mật khẩu
        /// </summary>
        /// <returns></returns>
        [HttpPut("change-password")]
        [ProducesResponseType(typeof(ApiResponse<User>), (int)HttpStatusCode.OK)]
        //[PermissionFilter(PermissionKeys.UserButtonAccountManagerUpdatePassword)]
        public ApiResponse ChangePassword(ChangePasswordDto input)
        {
            try
            {
                _userService.ChangePassword(input);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Set password cho user
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("set-password")]
        [ProducesResponseType(typeof(ApiResponse<User>), (int)HttpStatusCode.OK)]
        [PermissionFilter(PermissionKeys.UserButtonAccountManagerUpdatePassword)]
        public ApiResponse SetPassword(SetPasswordUserDto input)
        {
            try
            {
                _userService.SetPassword(input);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Lấy thông tin lần đăng nhập gần nhất + thiết bị đăng nhập
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-privacy-info")]
        public async Task<ApiResponse> GetPrivacyInfo()
        {
            try
            {
                return new(await _userService.GetPrivacyInfo());
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Cập nhật avatar tài khoản
        /// </summary>
        /// <param name="s3Key"></param>
        /// <returns></returns>
        [HttpPut("update-avatar")]
        public async Task<ApiResponse> UpdateAvatar([FromQuery] string s3Key)
        {
            try
            {
                await _userService.UpdateAvatar(s3Key);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Cập nhật role theo userId
        /// </summary>
        /// <returns></returns>
        [HttpPut("update-role")]
        [PermissionFilter(PermissionKeys.UserButtonAccountManagerUpdatePermission)]
        public ApiResponse UpdateRole(UpdateRoleDto input)
        {
            try
            {
                _userService.UpdateRole(input);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}
