using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopDev.Authentication.Domain.Users;
using ShopDev.Common.Filters;
using ShopDev.Constants.RolePermission.Constant;
using ShopDev.Inventory.ApplicationServices.CategoryModule.Abstracts;
using ShopDev.Inventory.ApplicationServices.CategoryModule.Dtos;
using ShopDev.Inventory.ApplicationServices.ShopModule.Abstracts;
using ShopDev.Inventory.ApplicationServices.ShopModule.Dtos;
using ShopDev.UserRolePermission;
using ShopDev.Utils.Net.Request;
using ShopDev.WebAPIBase.Controllers;

namespace ShopDev.Inventory.API.Controllers
{
    //[Authorize]
    //[AuthorizeAdminUserTypeFilter]
    [Route("api/inventory/shop")]
    [ApiController]
    public class ShopController : ApiControllerBase
    {
        private readonly IShopService _shopService;

        public ShopController(ILogger<ShopController> logger, IShopService shopService)
            : base(logger)
        {
            _shopService = shopService;
        }

        [HttpGet("find-all")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [PermissionFilter(PermissionKeys.UserTableAccountManager)]
        public ApiResponse FindAll()
        {
            try
            {
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<User>), (int)HttpStatusCode.OK)]
        //[PermissionFilter()]
        public async Task<ApiResponse> FindById(int id)
        {
            try
            {
                return new(await _shopService.FindByIdAsync(id));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [HttpPost("add")]
        [ProducesResponseType(typeof(ApiResponse<User>), (int)HttpStatusCode.OK)]
        //[PermissionFilter()]
        public ApiResponse Create(ShopCreateDto input)
        {
            try
            {
                _shopService.Create(input);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [HttpPut("update")]
        [ProducesResponseType(typeof(ApiResponse<User>), (int)HttpStatusCode.OK)]
        //[PermissionFilter(PermissionKeys.UserUpdate)]
        public ApiResponse Update()
        {
            try
            {
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [HttpPut("delete/{id}")]
        [ProducesResponseType(typeof(ApiResponse<User>), (int)HttpStatusCode.OK)]
        [PermissionFilter(PermissionKeys.UserButtonAccountManagerDelete)]
        public ApiResponse Delete(int id)
        {
            try
            {
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}
