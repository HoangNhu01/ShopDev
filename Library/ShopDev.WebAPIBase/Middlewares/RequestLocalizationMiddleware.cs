using ShopDev.ApplicationBase.Localization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ShopDev.WebAPIBase.Middlewares
{
    public class RequestLocalizationMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLocalizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var localizeQuery = context.Request.Query[LocalizationQuery.QueryName];
            if (!string.IsNullOrWhiteSpace(localizeQuery))
            {
                context.Items[LocalizationQuery.QueryName] = localizeQuery;
            }
            else
            {
                context.Items[LocalizationQuery.QueryName] = LocalizationBase.DicNameDefault;
            }
            // Call the next delegate/middleware in the pipeline.
            await _next(context);
        }
    }

    public static class RequestLocalizationMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLocalizationCustom(
            this IApplicationBuilder builder
        )
        {
            return builder.UseMiddleware<RequestLocalizationMiddleware>();
        }
    }
}
