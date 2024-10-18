using System.Text.Json;
using ShopDev.ApplicationBase.Localization;
using ShopDev.Constants.ErrorCodes;
using ShopDev.InfrastructureBase.Exceptions;
using ShopDev.Utils.Net.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ShopDev.Common.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<
                ILogger<ExceptionFilter>
            >();
            var mapErrorCode =
                context.HttpContext.RequestServices.GetRequiredService<IMapErrorCode>();
            var localization =
                context.HttpContext.RequestServices.GetRequiredService<LocalizationBase>();

            var request = context.HttpContext.Request;
            string errStr =
                $"Path = {request.Path}, Query = {JsonSerializer.Serialize(request.Query)}";
            int errorCode;

            string message = context.Exception.Message;
            if (context.Exception is UserFriendlyException userFriendlyException)
            {
                errorCode = userFriendlyException.ErrorCode;
                try
                {
                    message = !string.IsNullOrWhiteSpace(userFriendlyException.MessageLocalize)
                        ? localization.Localize(userFriendlyException.MessageLocalize)
                        : mapErrorCode.GetErrorMessage(userFriendlyException.ErrorCode);
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
                logger?.LogInformation(
                    context.Exception,
                    $"{context.Exception.GetType()}: {errStr}, ErrorCode = {errorCode}, Message = {message}"
                );
            }
            else
            {
                errorCode = ErrorCode.InternalServerError;
                logger?.LogError(
                    context.Exception,
                    $"{context.Exception.GetType()}: {errStr}, ErrorCode = {errorCode}, Message = {message}"
                );
                message = "Internal server error";
            }
            ApiResponse response =
                new(StatusCode.Error, nameof(ExceptionFilter), errorCode, message);
            context.Result = new JsonResult(response);
        }
    }
}
