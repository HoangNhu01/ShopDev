using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ShopDev.PaymentTool.Interfaces;

namespace ShopDev.PaymentTool.Configs
{
    public static class PaymentStartUp
    {
        public static void ConfigurePaymentTool(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<PaymentConfig>(
                builder.Configuration.GetSection("PaymentConfig")
            );
            builder.Services.AddScoped<IPaymentToolService, PaymentToolService>();
        }
    }
}
