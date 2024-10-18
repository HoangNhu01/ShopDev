using System.Text.Json;
using ShopDev.Constants.ErrorCodes;
using ShopDev.Utils.Net.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ShopDev.WebAPIBase.Filters
{
    public class CustomValidationErrorAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            ILogger? logger =
                context.HttpContext.RequestServices.GetRequiredService(
                    typeof(ILogger<CustomValidationErrorAttribute>)
                ) as ILogger;
            var request = context.HttpContext.Request;
            string errStr =
                $"Bad Request: Path = {request.Path}, Query = {JsonSerializer.Serialize(request.Query)}, ";

            if (
                context.Result is BadRequestObjectResult badRequestObjectResult
                && badRequestObjectResult.Value != null
                && badRequestObjectResult.Value is ValidationProblemDetails
            )
            {
                var errorFull = ToDictionary(badRequestObjectResult.Value);
                errorFull.TryGetValue("Errors", out object? errors);
                logger?.LogWarning($"{errStr} Errors: {JsonSerializer.Serialize(errors)}");
                var dicErrors = errors as Dictionary<string, string[]>;
                context.Result = new OkObjectResult(
                    new ApiResponse(
                        StatusCode.Error,
                        dicErrors?.Where(d => d.Key != "$"),
                        ErrorCode.BadRequest,
                        "Bad Request"
                    )
                );
            }

            base.OnResultExecuting(context);
        }

        private static Dictionary<string, object?> ToDictionary(object input)
        {
            return input
                .GetType()
                .GetProperties()
                .ToDictionary(
                    propInfo => propInfo.Name,
                    propInfo => propInfo.GetValue(input, null)
                );
        }
    }
}
