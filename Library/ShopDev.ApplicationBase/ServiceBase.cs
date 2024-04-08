using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ShopDev.ApplicationBase.Localization;

namespace ShopDev.ApplicationBase
{
    public abstract class ServiceBase<TDbContext>
        where TDbContext : DbContext
    {
        protected readonly ILogger _logger;
        protected readonly TDbContext _dbContext;
        protected readonly LocalizationBase _localization;
        protected readonly IHttpContextAccessor _httpContext;
        protected readonly IMapper _mapper;
        protected readonly IMapErrorCode? _mapErrorCode;

        protected ServiceBase(ILogger logger, IHttpContextAccessor httpContext)
        {
            _logger = logger;
            _httpContext = httpContext;
            _dbContext = httpContext.HttpContext!.RequestServices.GetRequiredService<TDbContext>();
            _localization =
                httpContext.HttpContext!.RequestServices.GetRequiredService<LocalizationBase>();
            _mapper = _httpContext.HttpContext!.RequestServices.GetRequiredService<IMapper>();
        }

        protected ServiceBase(
            ILogger logger,
            IHttpContextAccessor httpContext,
            TDbContext dbContext,
            LocalizationBase localizationBase,
            IMapper mapper
        )
        {
            _logger = logger;
            _httpContext = httpContext;
            _dbContext = dbContext;
            _localization = localizationBase;
            _mapper = mapper;
        }

        protected ServiceBase(
            ILogger logger,
            IMapErrorCode mapErrorCode,
            IHttpContextAccessor httpContext,
            TDbContext dbContext,
            LocalizationBase localizationBase,
            IMapper mapper
        )
        {
            _logger = logger;
            _httpContext = httpContext;
            _dbContext = dbContext;
            _localization = localizationBase;
            _mapper = mapper;
            _mapErrorCode = mapErrorCode;
        }

        /// <summary>
        /// Dịch sang ngôn ngữ đích dựa theo keyName và request ngôn ngữ là gì <br/>
        /// Input: <paramref name="keyName"/> = "error_System" <br/>
        /// Return: "Error System" hoặc "Lỗi" tuỳ theo request ngôn ngữ đang là gì ví dụ ở đây là "en" và "VI"
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        protected string L(string keyName)
        {
            return _localization.Localize(keyName);
        }

        /// <summary>
        /// Dịch sang ngôn ngữ đích dựa theo keyName và request ngôn ngữ là gì và dùng <c>string.Format()</c> để format chuỗi<br/>
        /// Ví dụ có thẻ <c>&lt;text name="hello"&gt;Xin chào {0}, {1} tuổi&lt;/text&gt;</c> trong file <c>xml</c> <br/>
        /// Input: <paramref name="keyName"/> = "hello" <paramref name="values"/> = ["Minh", 20] <br/>
        /// Return: "Xin chào Minh, 20 tuổi"
        /// </summary>
        /// <returns></returns>
        protected string L(string keyName, params string[] values)
        {
            return string.Format(_localization.Localize(keyName), values);
        }
    }
}
