using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ShopDev.WebAPIBase.Controllers
{
    /// <summary>
    /// Base controller, xử lý ngoại lệ
    /// </summary>
    public class BaseController : ControllerBase
    {
        protected ILogger? _logger;

        public BaseController(ILogger<BaseController> logger)
        {
            _logger = logger;
        }
    }
}
