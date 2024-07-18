using System.Net;
using Microsoft.AspNetCore.Mvc;
using ShopDev.Common.Filters;
using ShopDev.Constants.RolePermission.Constant;
using ShopDev.Order.ApplicationServices.OrderModule.Abstracts;
using ShopDev.Order.ApplicationServices.OrderModule.Dtos;
using ShopDev.Utils.Net.Request;
using ShopDev.WebAPIBase.Controllers;

namespace ShopDev.Order.API.Controllers
{
    //[Authorize]
    //[AuthorizeAdminUserTypeFilter]
    [Route("api/order/order-detail")]
    [ApiController]
    public class OrderController : ApiControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(ILogger<OrderController> logger, IOrderService orderService)
            : base(logger)
        {
            _orderService = orderService;
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
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [HttpPost("add")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        //[PermissionFilter()]
        public ApiResponse Create(OrderCreateDto input)
        {
            try
            {
                _orderService.Create(input);
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
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
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
