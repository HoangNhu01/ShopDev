using System.Net;
using Microsoft.AspNetCore.Mvc;
using ShopDev.Common.Filters;
using ShopDev.Constants.RolePermission.Constant;
using ShopDev.Order.API.Models;
using ShopDev.Order.ApplicationServices.OrderModule.Abstracts;
using ShopDev.Order.ApplicationServices.OrderModule.Dtos;
using ShopDev.PaymentTool.Interfaces;
using ShopDev.Utils.Net.Request;
using ShopDev.WebAPIBase.Controllers;

namespace ShopDev.Order.API.Controllers
{
    //[Authorize]
    //[AuthorizeAdminUserTypeFilter]
    [Route("api/order/payment")]
    [ApiController]
    public class PaymentController : Controller
    {
        private readonly IPaymentToolService _paymentToolService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(
            ILogger<PaymentController> logger,
            IPaymentToolService paymentToolService
        )
        {
            _logger = logger;
            _paymentToolService = paymentToolService;
        }

        [HttpPost("confirm-bill")]
        //[PermissionFilter()]
        public IActionResult ConfirmBill()
        {
            return View(new ConfirmBillModel { Message = "Thành công", IsSuccess = true });
        }
    }
}
