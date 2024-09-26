using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ShopDev.WebAPIBase.Minimal
{
    public static class MinimalEndpoints
    {
        public static void MapEndpoints(this WebApplication app)
        {
            app.MapMethods(
                "/consul/health",
                [HttpMethods.Get, HttpMethods.Head],
                () =>
                {
                    return Results.Ok();
                }
            );
        }
    }
}
