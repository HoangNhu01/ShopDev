using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using eShopSolution.ViewModels.Sales;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ShopDev.ApplicationBase.Localization;
using ShopDev.Common;
using ShopDev.Constants.ErrorCodes;
using ShopDev.Order.API.Models;
using ShopDev.PaymentTool.Configs;
using ShopDev.PaymentTool.Interfaces;
using ShopDev.Utils.Net.MimeTypes;
using ShopDev.Utils.Net.Request;
using ShopDev.Utils.Security;

namespace ShopDev.Order.API.Controllers
{
    //[Authorize]
    //[AuthorizeAdminUserTypeFilter]
    [Route("/vnpay")]
    [ApiController]
    public class PaymentController : Controller
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly IPaymentToolService _paymentToolService;
        private readonly PaymentConfig _config;
        private readonly LocalizationBase _localization;
        private readonly IHttpContextAccessor _httpContext;

        public PaymentController(
            ILogger<PaymentController> logger,
            IPaymentToolService paymentToolService,
            IOptions<PaymentConfig> options,
            LocalizationBase localization,
            IHttpContextAccessor httpContext
        )
        {
            _logger = logger;
            _paymentToolService = paymentToolService;
            _config = options.Value;
            _localization = localization;
            _httpContext = httpContext;
        }

        [HttpGet("confirm-bill")]
        //[PermissionFilter()]
        public IActionResult ConfirmBill()
        {
            ConfirmBillModel confirmBill = new();
            if (Request.Query.Count > 0)
            {
                string hashSecret = _config.HashSecret;
                var vnpayData = Request.Query.ToHashSet();
                //lấy toàn bộ dữ liệu được trả về
                foreach (var s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s.Key) && s.Key.StartsWith("vnp_"))
                    {
                        _paymentToolService.AddResponseData(s.Key, s.Value);
                    }
                }
                //mã hóa đơn
                string? orderId = _paymentToolService.GetResponseDataByKey("vnp_TxnRef");
                //mã giao dịch tại hệ thống VNPAY
                string? vnp_TranId = _paymentToolService.GetResponseDataByKey("vnp_TransactionNo");
                //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string? vnp_ResponseCode = _paymentToolService.GetResponseDataByKey(
                    "vnp_ResponseCode"
                );
                //hash của dữ liệu trả về
                string vnp_SecureHash = Request.Query["vnp_SecureHash"]!;
                //check chữ ký đúng hay không?
                bool checkSignature = CryptographyUtils.ValidateSignature(
                    vnp_SecureHash,
                    hashSecret,
                    _paymentToolService.GetResponseData()
                );

                if (checkSignature && vnp_ResponseCode == "00")
                {
                    //Thanh toán thành công
                    confirmBill.Message = _localization.Localize(
                        "success_TransactionConfirm",
                        [orderId!, vnp_TranId!]
                    );
                    confirmBill.IsSuccess = true;
                }
                else
                {
                    //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                    confirmBill.Message = _localization.Localize(
                        "error_TransactionError",
                        [orderId!, vnp_TranId!, vnp_ResponseCode!]
                    );
                    confirmBill.IsSuccess = false;
                }
            }
            else
            {
                confirmBill.IsSuccess = false;
            }
            //await _hubContext.Clients.All.SendAsync("StatusOrderMessage", payMentStatus);
            return View(confirmBill);
        }

        [HttpPost("get-url")]
        public IActionResult PaymentRequest([FromBody] GetUrlModel input)
        {
            //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
            _paymentToolService.AddRequestData("vnp_Version", "2.1.0");
            //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            _paymentToolService.AddRequestData("vnp_Command", "pay");
            //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            _paymentToolService.AddRequestData("vnp_TmnCode", _config.TmnCode);
            //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
            _paymentToolService.AddRequestData("vnp_Amount", (input.TotalPrice * 100).ToString());
            //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/),
            //có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            _paymentToolService.AddRequestData("vnp_BankCode", string.Empty);
            //ngày thanh toán theo định dạng yyyyMMddHHmmss
            _paymentToolService.AddRequestData(
                "vnp_CreateDate",
                DateTime.Now.ToString("yyyyMMddHHmmss")
            );
            //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VN
            _paymentToolService.AddRequestData("vnp_CurrCode", "VND");
            //Địa chỉ IP của khách hàng thực hiện giao dịch
            _paymentToolService.AddRequestData(
                "vnp_IpAddr",
                _httpContext.GetCurrentRemoteIpAddress()
            );
            //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            _paymentToolService.AddRequestData("vnp_Locale", "vn");
            //Thông tin mô tả nội dung thanh toán
            _paymentToolService.AddRequestData("vnp_OrderInfo", input.OrderId.ToString());
            //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            _paymentToolService.AddRequestData("vnp_OrderType", "other");
            //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            _paymentToolService.AddRequestData("vnp_ReturnUrl", _config.ReturnUrl);
            //mã hóa đơn
            _paymentToolService.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString());
            string paymentUrl = _paymentToolService.CreateRequestUrl(
                _config.Url,
                _config.HashSecret
            );
            return Ok(new ApiResponse { Data = paymentUrl });
        }

        //[Authorize]
        [HttpPost("refund")]
        public async Task<IActionResult> RefundTransaction([FromBody] RefundRequestModel request)
        {
            var vnp_Version = "2.1.0";
            var vnp_Command = "refund";
            var vnp_TransactionType = "02"; // 02 for refund
            var vnp_RequestId = Guid.NewGuid().ToString(); // Unique request ID
            var vnp_CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss"); // Format the create date
            var ip = _httpContext.GetCurrentRemoteIpAddress();
            var vnp_Amount = ((request.Amount * 100)).ToString(); // VNPay expects the amount in cents

            // Construct the sign data
            var signData =
                $"{vnp_RequestId}|{vnp_Version}"
                + $"|{vnp_Command}|{_config.TmnCode}"
                + $"|{vnp_TransactionType}|{request.TransactionId}"
                + $"|{vnp_Amount}|{string.Empty}|{vnp_CreateDate}|{"DrinkShop"}"
                + $"|{vnp_CreateDate}|{ip}|{string.Empty}";

            // Generate the checksum
            string vnp_SecureHash = CryptographyUtils.HmacSHA512(_config.HashSecret, signData);

            // Construct the request data
            var rfData = new
            {
                vnp_RequestId,
                vnp_Version,
                vnp_Command,
                vnp_TmnCode = _config.TmnCode,
                vnp_TransactionType,
                vnp_TxnRef = request.TransactionId,
                vnp_Amount,
                vnp_OrderInfo = string.Empty,
                vnp_TransactionNo = string.Empty,
                vnp_TransactionDate = vnp_CreateDate,
                vnp_CreateBy = "DrinkShop",
                vnp_CreateDate,
                vnp_IpAddr = ip,
                vnp_SecureHash
            };

            // Send the request to VNPay
            using HttpClient httpClient = new();
            var content = new StringContent(
                JsonSerializer.Serialize(rfData),
                Encoding.UTF8,
                MimeTypeNames.ApplicationJson
            );
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue(MimeTypeNames.ApplicationJson)
            );

            var response = await httpClient.PostAsync(_config.RefundUrl, content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return Ok(responseContent);
            }
            else
            {
                return Ok(new ApiResponse { Code = ErrorCode.BadRequest, Message = "Bad Request" });
            }
        }
    }
}
