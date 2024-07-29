using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ShopDev.ApplicationBase.Localization;
using ShopDev.Order.ApplicationServices.Common;
using ShopDev.Order.ApplicationServices.PaymentModule.Abstracts;
using ShopDev.Order.Infrastructure.Persistence;

namespace ShopDev.Order.ApplicationServices.PaymentModule.Implements
{
    public class PaymentService : OrderServiceBase, IPaymentService
    {
        public PaymentService(
            ILogger<PaymentService> logger,
            IHttpContextAccessor httpContext,
            OrderDbContext dbContext,
            LocalizationBase localizationBase,
            IMapper mapper
        )
            : base(logger, httpContext, dbContext, localizationBase, mapper) { }

    }
}
