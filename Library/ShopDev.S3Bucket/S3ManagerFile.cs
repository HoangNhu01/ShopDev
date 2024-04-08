using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ShopDev.S3Bucket.Configs;
using ShopDev.S3Bucket.Constants;
using ShopDev.S3Bucket.Dtos.Delete;
using ShopDev.S3Bucket.Dtos.Media;
using ShopDev.S3Bucket.Dtos.Move;
using ShopDev.S3Bucket.Exceptions;
using ShopDev.Utils.Net.Request;

namespace ShopDev.S3Bucket
{
    public class S3ManagerFile : IS3ManagerFile
    {
        private readonly ILogger _logger;
        private readonly S3Config _config;

        public S3ManagerFile(ILogger<S3ManagerFile> logger, IOptions<S3Config> config)
        {
            _logger = logger;
            _config = config.Value;
        }

        public async Task<ResponseContentDto> DeleteFileAsync(RequestDeleteDto input)
        {
            _logger.LogInformation(
                $"{nameof(DeleteFileAsync)}: input = {JsonSerializer.Serialize(input)}"
            );
            if (input.S3Key == null || input.FolderType == null)
            {
                return new();
            }
            using HttpClient client = new();
            client.BaseAddress = new Uri(_config.BaseUrl);
            client.DefaultRequestHeaders.Add(
                S3Config.XClientSourceHeader,
                S3Config.XClientSourceValue
            );
            // Gửi HTTP DELETE request
            HttpResponseMessage response = await client.DeleteAsJsonAsync(
                S3Config.MediaPath,
                input
            );
            var responseBody = await response.Content.ReadFromJsonAsync<ResponseContentDto>();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(
                    $"{nameof(DeleteFileAsync)}: responseBody = {await response.Content.ReadAsStringAsync()}, responseStatusCode = {response.StatusCode}"
                );
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new S3BucketException(S3ManagerFileErrorCode.DeleteMediaNotFound);
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    if (responseBody != null && responseBody.Message != null)
                    {
                        throw new S3BucketException(responseBody.Message);
                    }
                    throw new S3BucketException(S3ManagerFileErrorCode.DeleteMediaBadRequest);
                }
                else
                {
                    throw new S3BucketException(S3ManagerFileErrorCode.DeleteMediaError);
                }
            }
            else
            {
                if (
                    responseBody != null
                    && responseBody.Code == StatusCodes.Status500InternalServerError
                )
                {
                    throw new S3BucketException(S3ManagerFileErrorCode.DeleteMediaError);
                }
                else if (
                    responseBody != null
                    && responseBody.Code == StatusCodes.Status400BadRequest
                )
                {
                    if (responseBody.Message != null)
                    {
                        throw new S3BucketException(responseBody.Message);
                    }
                    throw new S3BucketException(S3ManagerFileErrorCode.DeleteMediaBadRequest);
                }
                else if (responseBody != null && responseBody.Code == StatusCodes.Status404NotFound)
                {
                    throw new S3BucketException(S3ManagerFileErrorCode.DeleteMediaNotFound);
                }
                else if (
                    responseBody != null
                    && responseBody.Code == StatusCodes.Status401Unauthorized
                )
                {
                    throw new S3BucketException(S3ManagerFileErrorCode.DeleteMediaError);
                }
                return responseBody ?? new();
            }
        }

        public async Task<Stream> ReadMediaAsync(string path)
        {
            _logger.LogInformation($"{nameof(ReadMediaAsync)}: path = {path}");
            using HttpClient httpClient = new HttpClient();
            var uri = new Uri($"{_config.ViewMediaUrl}{path}");
            var response = await httpClient.GetAsync(uri);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var errorMessage = await response.Content.ReadFromJsonAsync<ResponseContentDto>();
                //Xử lý các ngoại lệ lúc đọc file từ server
                _logger.LogError(
                    $"{nameof(ReadMediaAsync)}: responseBody = {await response.Content.ReadAsStringAsync()}, responseStatusCode = {response.StatusCode}"
                );
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new S3BucketException(S3ManagerFileErrorCode.ReadMediaNotFound);
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    if (errorMessage != null && errorMessage.Message != null)
                    {
                        throw new S3BucketException(errorMessage.Message);
                    }
                    throw new S3BucketException(S3ManagerFileErrorCode.ReadMediaBadRequest);
                }
                else
                {
                    throw new S3BucketException(S3ManagerFileErrorCode.ReadMediaError);
                }
            }
            return await response.Content.ReadAsStreamAsync();
        }

        public async Task<ResponseMoveDto> WriteFileAsync(params IFormFile[] files)
        {
            _logger.LogInformation(
                $"{nameof(WriteFileAsync)}: files = [{string.Join("; ", files?.Select(i => $"fileName = {i?.FileName}, length = {i?.Length:N2} byte") ?? [])}]"
            );
            if (files == null || files.Length == 0)
            {
                return new();
            }
            await Task.CompletedTask;
            throw new NotImplementedException();
        }

        public async Task<ResponseMoveDto> WriteStreamFileAsync(params S3StreamFile[] files)
        {
            _logger.LogInformation(
                $"{nameof(WriteStreamFileAsync)}: files = [{string.Join("; ", files?.Select(i => $"fileName = {i?.FileName}, length = {i?.StreamFiles?.Length:N2} byte") ?? [])}]"
            );
            if (files == null)
            {
                return new();
            }
            await Task.CompletedTask;
            throw new NotImplementedException();
        }
    }
}
