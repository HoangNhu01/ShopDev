using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopDev.ApplicationBase.Common.Validations;
using ShopDev.Authentication.ApplicationServices.AuthenticationModule.Abstract;
using ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.PermissionDto;
using ShopDev.Common.Filters;
using ShopDev.Constants.Domain.Auth.Role;
using ShopDev.Constants.RolePermission.Constant;
using ShopDev.Utils.Net.Request;
using ShopDev.WebAPIBase.Controllers;

namespace ShopDev.Authentication.API.Controllers
{
    [Authorize]
    [Route("api/auth/permission")]
    [ApiController]
    public class PermissionController : ApiControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(
            ILogger<PermissionController> logger,
            IPermissionService permissionService
        )
            : base(logger)
        {
            _permissionService = permissionService;
        }

        /// <summary>
        /// Xem danh quyền
        /// </summary>
        /// <returns></returns>
        [HttpGet("find-all/{permissionConfig}")]
        [ProducesResponseType(
            typeof(ApiResponse<IEnumerable<PermissionDetailDto>>),
            (int)HttpStatusCode.OK
        )]
        [PermissionFilter(
            PermissionKeys.UserButtonRolePermissionUpdate,
            PermissionKeys.UserButtonPermissionAdd
        )]
        public ApiResponse FindAll(
            [IntegerRange(
                AllowableValues = new int[]
                {
                    PermissionInWebs.Home,
                    PermissionInWebs.User,
                    PermissionInWebs.Core,
                    PermissionInWebs.Saler,
                    PermissionInWebs.Invest,
                }
            )]
                int permissionConfig
        )
        {
            try
            {
                return new ApiResponse(_permissionService.FindAllPermission(permissionConfig));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Lấy permissionKey của người dùng hiện tại
        /// </summary>
        /// <param name="permissionInWeb">Website cần lấy permission</param>
        /// <returns></returns>
        [HttpGet("get-permissions")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<string>>), (int)HttpStatusCode.OK)]
        public ApiResponse GetAllPermission(int? permissionInWeb)
        {
            try
            {
                return new(_permissionService.GetPermissionInWeb(permissionInWeb));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Lấy permissionKey của người dùng bất kì
        /// </summary>
        /// <param name="permissionInWeb">Website cần lấy permission</param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("get-permissions-internal")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<string>>), (int)HttpStatusCode.OK)]
        public ApiResponse GetPermissionInternalService(int? permissionInWeb, int userId)
        {
            try
            {
                return new(
                    _permissionService.GetPermissionInternalService(permissionInWeb, userId)
                );
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Lấy số nhóm quyền, số người dùng theo website
        /// </summary>
        /// <returns></returns>
        [HttpGet("find-by-permission-in-web")]
        [ProducesResponseType(
            typeof(ApiResponse<IEnumerable<PermissionInWebDto>>),
            (int)HttpStatusCode.OK
        )]
        [PermissionFilter(PermissionKeys.UserTablePermission)]
        public ApiResponse FindByPermissionInWeb()
        {
            try
            {
                return new(_permissionService.FindByPermissionInWeb());
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}
