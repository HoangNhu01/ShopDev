using Microsoft.AspNetCore.Mvc;
using ShopDev.Authentication.Domain.Users;
using ShopDev.Common.Filters;
using ShopDev.Constants.RolePermission.Constant;
using ShopDev.Inventory.ApplicationServices.CategoryModule.Abstracts;
using ShopDev.Inventory.ApplicationServices.CategoryModule.Dtos;
using ShopDev.Utils.Net.Request;
using ShopDev.WebAPIBase.Controllers;
using System.Net;

namespace ShopDev.Authentication.API.Controllers
{
    //[Authorize]
    //[AuthorizeAdminUserTypeFilter]
    [Route("api/inventory/category")]
    [ApiController]
    public class CategoryController : ApiControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(
            ILogger<CategoryController> logger,
            ICategoryService categoryService
        )
            : base(logger)
        {
            _categoryService = categoryService;
        }

        [HttpGet("find-all")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        //[PermissionFilter(PermissionKeys.UserTableAccountManager)]
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

        [HttpGet("find-by-id/{id}")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        //[PermissionFilter()]
        public ApiResponse FindById(int id)
        {
            try
            {
                return new(_categoryService.FindById(id));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [HttpPost("add")]
        [ProducesResponseType(typeof(ApiResponse<User>), (int)HttpStatusCode.OK)]
        //[PermissionFilter()]
        public ApiResponse Create(CategoryCreateDto input)
        {
            try
            {
                _categoryService.Create(input);
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
