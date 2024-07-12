using Microsoft.AspNetCore.Mvc;
using ShopDev.Inventory.ApplicationServices.ProductModule.Abstract;
using ShopDev.Inventory.ApplicationServices.ProductModule.Dtos;
using ShopDev.Utils.Net.Request;
using ShopDev.WebAPIBase.Controllers;
using System.Net;

namespace ShopDev.Inventory.API.Controllers
{
    //[Authorize]
    [Route("api/inventory/product")]
    [ApiController]
    public class ProductController : ApiControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(ILogger<ProductController> logger, IProductService productService)
            : base(logger)
        {
            _productService = productService;
        }

        [HttpGet("find-all")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        //[PermissionFilter(PermissionKeys.UserTableAccountManager)]
        public ApiResponse FindAll(ProductFilterDto input)
        {
            try
            {
                return new(_productService.FindAll(input));
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
                return new(_productService.FindById(id));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [HttpPost("add")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        //[PermissionFilter()]
        public ApiResponse Create(ProductCreateDto input)
        {
            try
            {
                _productService.Create(input);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [HttpPut("update")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        //[PermissionFilter(PermissionKeys.UserUpdate)]
        public ApiResponse Update(ProductUpdateDto input)
        {
            try
            {
                _productService.Update(input);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [HttpPut("delete/{id}")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        //[PermissionFilter(PermissionKeys.UserButtonAccountManagerDelete)]
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
