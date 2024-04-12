using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ShopDev.ApplicationBase.Localization;
using ShopDev.Constants.ErrorCodes;
using ShopDev.ConvertFile;
using ShopDev.ConvertFile.Exceptions;
using ShopDev.ConvertFile.Localization;
using ShopDev.InfrastructureBase.Exceptions;
using ShopDev.S3Bucket;
using ShopDev.S3Bucket.Exceptions;
using ShopDev.S3Bucket.Localization;
using ShopDev.Utils.Net.File;
using ShopDev.Utils.Net.MimeTypes;
using ShopDev.Utils.Net.Request;

namespace ShopDev.WebAPIBase.Controllers
{
    public class ApiControllerBase : ControllerBase
    {
        protected ILogger _logger;

        public ApiControllerBase(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Return file với stream file
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [NonAction]
        protected FileStreamResult FileByStream(Stream fileStream, string fileName)
        {
            string? ext = Path.GetExtension(fileName)?.ToLower();

            return ext switch
            {
                FileTypes.JPG
                or FileTypes.JPEG
                or FileTypes.JFIF
                    => File(fileStream, MimeTypeNames.ImageJpeg),
                FileTypes.PNG => File(fileStream, MimeTypeNames.ImagePng),
                FileTypes.SVG => File(fileStream, MimeTypeNames.ImageSvgXml),
                FileTypes.GIF => File(fileStream, MimeTypeNames.ImageGif),
                FileTypes.MP4 => File(fileStream, MimeTypeNames.VideoMp4),
                FileTypes.PDF => File(fileStream, MimeTypeNames.ApplicationPdf),
                FileTypes.WEBP => File(fileStream, MimeTypeNames.ImageWebp),
                _ => File(fileStream, MimeTypeNames.ApplicationOctetStream, fileName),
            };
        }

        /// <summary>
        /// Return file với byte file
        /// </summary>
        /// <param name="fileByte"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [NonAction]
        protected FileContentResult FileByFormat(byte[] fileByte, string fileName)
        {
            string? ext = Path.GetExtension(fileName)?.ToLower();

            return ext switch
            {
                FileTypes.JPG
                or FileTypes.JPEG
                or FileTypes.JFIF
                    => File(fileByte, MimeTypeNames.ImageJpeg),
                FileTypes.PNG => File(fileByte, MimeTypeNames.ImagePng),
                FileTypes.SVG => File(fileByte, MimeTypeNames.ImageSvgXml),
                FileTypes.GIF => File(fileByte, MimeTypeNames.ImageGif),
                FileTypes.MP4 => File(fileByte, MimeTypeNames.VideoMp4),
                FileTypes.PDF => File(fileByte, MimeTypeNames.ApplicationPdf),
                FileTypes.WEBP => File(fileByte, MimeTypeNames.ImageWebp),
                _ => File(fileByte, MimeTypeNames.ApplicationOctetStream, fileName),
            };
        }

        [NonAction]
        public ApiResponse OkException(Exception ex)
        {
            var mapErrorCode = HttpContext.RequestServices.GetRequiredService<IMapErrorCode>();
            var localization = HttpContext.RequestServices.GetRequiredService<LocalizationBase>();

            var request = HttpContext.Request;
            string errStr =
                $"Path = {request.Path}, Query = {JsonSerializer.Serialize(request.Query)}";
            int errorCode;
            string message = ex.Message;
            object? data = null;
            if (ex is UserFriendlyException userFriendlyException)
            {
                errorCode = userFriendlyException.ErrorCode;
                try
                {
                    if (!string.IsNullOrWhiteSpace(userFriendlyException.ErrorMessage))
                    {
                        message = userFriendlyException.ErrorMessage;
                    }
                    else if (
                        userFriendlyException.ListParam is not null
                        && userFriendlyException.ListParam.Length > 0
                    )
                    {
                        message = localization.Localize(
                            mapErrorCode.GetErrorMessageKey(errorCode),
                            userFriendlyException.ListParam
                        );
                    }
                    else
                    {
                        message = !string.IsNullOrWhiteSpace(userFriendlyException.MessageLocalize)
                            ? localization.Localize(userFriendlyException.MessageLocalize)
                            : mapErrorCode.GetErrorMessage(userFriendlyException.ErrorCode);
                    }
                }
                catch (Exception exc)
                {
                    message = exc.Message;
                }
                _logger?.LogInformation(
                    ex,
                    $"{ex.GetType()}: {errStr}, ErrorCode = {errorCode}, Message = {message}"
                );
            }
            else if (ex is DbUpdateException dbUpdateException)
            {
                errorCode = ErrorCode.InternalServerError;
                if (dbUpdateException.InnerException != null)
                {
                    message = dbUpdateException.InnerException.Message;
                }
                else
                {
                    message = dbUpdateException.Message;
                }
                _logger?.LogInformation(
                    ex,
                    $"{ex.GetType()}: {errStr}, ErrorCode = {errorCode}, Message = {message}"
                );
            }
            else if (ex is InvalidOperationException invalidOperationException)
            {
                errorCode = ErrorCode.InternalServerError;
                if (invalidOperationException.InnerException != null)
                {
                    message = invalidOperationException.InnerException.Message;
                }
                else
                {
                    message = invalidOperationException.Message;
                }
                _logger?.LogInformation(
                    ex,
                    $"{ex.GetType()}: {errStr}, ErrorCode = {errorCode}, Message = {message}"
                );
            }
            else if (ex is S3BucketException s3BucketException)
            {
                errorCode = s3BucketException.ErrorCode;
                try
                {
                    var s3MapErrorCode =
                        HttpContext.RequestServices.GetRequiredService<IS3MapErrorCode>();
                    var s3Localization =
                        HttpContext.RequestServices.GetRequiredService<IS3Localization>();

                    if (!string.IsNullOrWhiteSpace(s3BucketException.ErrorMessage))
                    {
                        message = s3BucketException.ErrorMessage;
                    }
                    else if (
                        s3BucketException.ListParam is not null
                        && s3BucketException.ListParam.Length > 0
                    )
                    {
                        message = localization.Localize(
                            mapErrorCode.GetErrorMessageKey(errorCode),
                            s3BucketException.ListParam
                        );
                    }
                    else
                    {
                        message = !string.IsNullOrWhiteSpace(s3BucketException.MessageLocalize)
                            ? s3Localization.Localize(s3BucketException.MessageLocalize)
                            : s3MapErrorCode.GetErrorMessage(s3BucketException.ErrorCode);
                    }
                }
                catch (Exception exc)
                {
                    message = exc.Message;
                }
            }
            else if (ex is ConvertFileException convertFileException)
            {
                errorCode = convertFileException.ErrorCode;
                try
                {
                    var convertFileMapError =
                        HttpContext.RequestServices.GetRequiredService<IConvertFileMapErrorCode>();
                    var convertFileLocalization =
                        HttpContext.RequestServices.GetRequiredService<IConvertFileLocalization>();

                    if (!string.IsNullOrWhiteSpace(convertFileException.ErrorMessage))
                    {
                        message = convertFileException.ErrorMessage;
                    }
                    else if (
                        convertFileException.ListParam is not null
                        && convertFileException.ListParam.Length > 0
                    )
                    {
                        message = localization.Localize(
                            mapErrorCode.GetErrorMessageKey(errorCode),
                            convertFileException.ListParam
                        );
                    }
                    else
                    {
                        message = !string.IsNullOrWhiteSpace(convertFileException.MessageLocalize)
                            ? convertFileLocalization.Localize(convertFileException.MessageLocalize)
                            : convertFileMapError.GetErrorMessage(convertFileException.ErrorCode);
                    }
                }
                catch (Exception exc)
                {
                    message = exc.Message;
                }
            }
            else
            {
                errorCode = ErrorCode.InternalServerError;
                _logger?.LogError(
                    ex,
                    $"{ex.GetType()}: {errStr}, ErrorCode = {errorCode}, Message = {message}"
                );
                message = "Internal server error";
            }
            //ApiResponse response = new(Utils.Net.Request.StatusCode.Error, nameof(OkException), errorCode, message);
            //ex.Result = new JsonResult(response);
            return new ApiResponse(Utils.Net.Request.StatusCode.Error, data, errorCode, message);
        }
    }
}
