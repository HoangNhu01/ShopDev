using System.Net;
using Microsoft.AspNetCore.Mvc;
using ShopDev.Order.ApplicationServices.CartModule.Abstract;
using ShopDev.Order.ApplicationServices.CartModule.Dtos;
using ShopDev.Utils.Net.Request;
using ShopDev.WebAPIBase.Controllers;

namespace ShopDev.Order.API.Controllers
{
    //[Authorize]
    [Route("api/order/cart")]
    [ApiController]
    public class CartController : ApiControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ILogger<CartController> logger, ICartService cartService)
            : base(logger)
        {
            _cartService = cartService;
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

        [HttpGet("view-cart")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        //[PermissionFilter()]
        public async Task<ApiResponse> ViewCart()
        {
            try
            {
                return new(await _cartService.ViewCart());
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [HttpPost("add")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        //[PermissionFilter()]
        public ApiResponse Create()
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

        [HttpPut("update")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        //[PermissionFilter(PermissionKeys.UserUpdate)]
        public async Task<ApiResponse> Update(CartUpdateDto input)
        {
            try
            {
                await _cartService.AddToCart(input);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [HttpDelete("delete")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        //[PermissionFilter(PermissionKeys.UserButtonAccountManagerDelete)]
        public async Task<ApiResponse> Delete(CartUpdateDto input)
        {
            try
            {
                await _cartService.RemoveFromCart(input);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}
