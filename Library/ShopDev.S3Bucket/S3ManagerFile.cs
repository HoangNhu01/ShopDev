using System.Net;
using System.Net.Http.Json;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ShopDev.Constants.File;
using ShopDev.InfrastructureBase.Files.Dtos;
using ShopDev.S3Bucket.Configs;
using ShopDev.S3Bucket.Constants;
using ShopDev.S3Bucket.Dtos.Media;
using ShopDev.S3Bucket.Exceptions;
using ShopDev.Utils.DataUtils;
using ShopDev.Utils.Net.File;

namespace ShopDev.S3Bucket
{
    public class S3ManagerFile : IS3ManagerFile
    {
        private readonly ILogger _logger;
        private readonly S3Config _config;
        private readonly IAmazonS3 _s3Client;

        private readonly string BucketName = S3Config.BucketName;

        public S3ManagerFile(ILogger<S3ManagerFile> logger, IOptions<S3Config> config)
        {
            _logger = logger;
            _config = config.Value;
            var credential = new BasicAWSCredentials(_config.AccessKey, _config.SecretKey);
            _s3Client = new AmazonS3Client(
                credential,
                new AmazonS3Config
                {
                    ServiceURL = _config.ServiceUrl,
                    ForcePathStyle = true, // Yêu cầu để làm việc với MinIO,
                }
            );
            //ConfigureLifecyclePolicyAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Cấu hình lifetime của file (tự động xóa sau 1 ngày)
        /// </summary>
        /// <returns></returns>
        public async Task ConfigureLifecyclePolicyAsync()
        {
            var lifecycleConfiguration = new LifecycleConfiguration
            {
                Rules =
                [
                    new()
                    {
                        Id = "DeleteOldObjects",
                        Filter = new LifecycleFilter
                        {
                            LifecycleFilterPredicate = new LifecyclePrefixPredicate
                            {
                                Prefix = "temp/"
                            }
                        },
                        Status = LifecycleRuleStatus.Enabled,
                        Expiration = new LifecycleRuleExpiration
                        {
                            // Số ngày sau đó đối tượng sẽ bị xóa
                            Days = 1
                        }
                    }
                ]
            };

            PutLifecycleConfigurationRequest putLifecycleRequest =
                new() { BucketName = S3Config.BucketName, Configuration = lifecycleConfiguration };

            try
            {
                await _s3Client.PutLifecycleConfigurationAsync(putLifecycleRequest);
                _logger.LogInformation(
                    $"{ConfigureLifecyclePolicyAsync}: Lifecycle policy đã được thiết lập thành công."
                );
            }
            catch (AmazonS3Exception e)
            {
                _logger.LogError(
                    $"{ConfigureLifecyclePolicyAsync}: Error encountered on server. Message:'{e.Message}' when writing an object"
                );
            }
            catch (Exception e)
            {
                _logger.LogError(
                    $"{ConfigureLifecyclePolicyAsync}: Unknown encountered on server. Message:'{e.Message}' when writing an object"
                );
            }
        }

        /// <summary>
        /// Check xem bucket có tồn tại không, nếu không sẽ tạo mới
        /// </summary>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        /// <exception cref="S3BucketException"></exception>
        private async Task S3BucketExistAsync(string bucketName)
        {
            var response = await _s3Client.ListBucketsAsync();
            if (!response.Buckets.Exists(x => x.BucketName == bucketName))
            {
                try
                {
                    //tạo bucket
                    var putBucketRequest = new PutBucketRequest
                    {
                        BucketName = bucketName,
                        UseClientRegion = true,
                    };
                    var responseCreateBucket = await _s3Client.PutBucketAsync(putBucketRequest);

                    //Access policy của bucket
                    string bucketPolicy =
                        @"{
                        ""Version"": ""2012-10-17"",
                        ""Statement"": [
                            {
                                ""Effect"": ""Allow"",
                                ""Principal"": ""*"",
                                ""Action"": ""s3:GetObject"",
                                ""Resource"": ""arn:aws:s3:::"
                        + bucketName
                        + @"/*""
                            }
                        ]
                    }";

                    var putPolicyRequest = new PutBucketPolicyRequest
                    {
                        BucketName = bucketName,
                        Policy = bucketPolicy
                    };
                    await _s3Client.PutBucketPolicyAsync(putPolicyRequest);
                }
                catch (AmazonS3Exception e)
                {
                    _logger.LogError(
                        $"{S3BucketExistAsync}: Error encountered on server. Message:'{e.Message}' when writing an object"
                    );
                    throw new S3BucketException(S3ManagerFileErrorCode.CreatBucketError);
                }
                catch (Exception e)
                {
                    _logger.LogError(
                        $"{S3BucketExistAsync}: Error encountered on server. Message:'{e.Message}' when writing an object"
                    );
                    throw new S3BucketException(
                        S3ManagerFileErrorCode.ErrorMessage,
                        listParam: e.Message
                    );
                }
            }
        }

        /// <summary>
        /// Tạo tiền tố của s3key
        /// </summary>
        /// <returns></returns>
        public static string GenerateObjectName()
        {
            DateTime currentDate = DateTime.Now;
            string year = currentDate.Year.ToString();
            string month = currentDate.Month.ToString("00"); // Đảm bảo định dạng 2 chữ số
            string day = currentDate.Day.ToString("00"); // Đảm bảo định dạng 2 chữ số

            // Tạo object name theo định dạng: yyyy/MM/dd
            string objectName = $"{year}/{month}/{day}";

            return objectName;
        }

        /// <summary>
        /// tạo mới s3 key khi move
        /// </summary>
        /// <param name="tempS3Key"></param>
        /// <returns></returns>
        private static string GenerateNewMoveS3Key(string tempS3Key)
        {
            // Define the marker string
            string marker = "temp/";

            // Find the index of the marker
            int index = tempS3Key.IndexOf(marker);

            // Check if the marker exists in the file path
            if (index != -1)
            {
                // Extract the substring starting just after the marker
                return tempS3Key.Substring(index + marker.Length);
            }
            else
            {
                // If the marker is not found, return an empty string or handle as needed
                return string.Empty;
            }
        }

        private string GetS3KeyForRemoveFile(string s3Key)
        {
            // Define the marker string
            string marker = $"{BucketName}/";

            // Find the index of the marker
            int index = s3Key.IndexOf(marker);

            // Check if the marker exists in the file path
            if (index != -1)
            {
                // Extract the substring starting just after the marker
                return s3Key.Substring(index + marker.Length);
            }
            else
            {
                // If the marker is not found, return an empty string or handle as needed
                return string.Empty;
            }
        }

        /// <summary>
        /// Lấy file name từ path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string GetFileNameFromPath(string path)
        {
            int lastSlashIndex = path.LastIndexOf('/');
            if (lastSlashIndex == -1)
            {
                return path;
            }
            else
            {
                return path.Substring(lastSlashIndex + 1);
            }
        }

        /// <summary>
        /// Map response
        /// </summary>
        /// <param name="s3Key"></param>
        /// <param name="contentType"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private ResponseUploadDto MapResponse(
            string oldS3Key,
            string s3Key,
            string contentType,
            string fileName
        )
        {
            return new()
            {
                Uri = $"{BucketName}/{s3Key}",
                S3Key = s3Key,
                OldS3Key = oldS3Key,
                MimeType = contentType,
                Name = fileName
            };
        }

        public async Task DeleteAsync(params string[] s3key)
        {
            _logger.LogInformation($"{nameof(DeleteAsync)}: input = {s3key}");
            if (s3key == null || !s3key.Any())
            {
                return;
            }
            try
            {
                foreach (var item in s3key)
                {
                    string key = GetS3KeyForRemoveFile(item);
                    var deleteObjectRequest = new DeleteObjectRequest
                    {
                        BucketName = BucketName,
                        Key = key
                    };
                    await _s3Client.DeleteObjectAsync(deleteObjectRequest);
                }
            }
            catch (AmazonS3Exception e)
            {
                _logger.LogError(
                    $"{DeleteAsync}: Error encountered on server. Message:'{e.Message}' when deleting an object"
                );
                throw new S3BucketException(S3ManagerFileErrorCode.DeleteMediaError);
            }
            catch (Exception e)
            {
                _logger.LogError(
                    $"{DeleteAsync}: Error encountered on server. Message:'{e.Message}' when deleting an object"
                );
                throw new S3BucketException(S3ManagerFileErrorCode.DeleteMediaBadRequest);
            }
        }

        public async Task<DownloadFileDto> DownloadAsync(string s3Key)
        {
            _logger.LogInformation($"{nameof(DownloadAsync)}: s3Key = {s3Key}");

            var result = new DownloadFileDto();
            await S3BucketExistAsync(BucketName);
            try
            {
                var getRequest = new GetObjectRequest { BucketName = BucketName, Key = s3Key };

                var response = await _s3Client.GetObjectAsync(getRequest);

                var fileName = GetFileNameFromPath(response.Key);
                var responseStream = response.ResponseStream;

                // Đọc metadata và headers
                string title = response.Metadata["x-amz-meta-title"];
                string contentType = response.Headers["Content-Type"];

                result.Stream = responseStream;
                result.ContentType = contentType;
                result.FileName = fileName;
                return result;
            }
            catch (AmazonS3Exception e)
            {
                _logger.LogError($"{DownloadAsync}: Lỗi gặp phải khi tải xuống tệp: {e.Message}");
                throw new S3BucketException(S3ManagerFileErrorCode.ReadMediaError);
            }
            catch (Exception e)
            {
                _logger.LogError($"{DownloadAsync}: Lỗi gặp phải khi tải xuống tệp: {e.Message}");
                throw new S3BucketException(S3ManagerFileErrorCode.ReadMediaError);
            }
        }

        public async Task<List<ResponseUploadDto>> MoveAsync(params string[] s3key)
        {
            _logger.LogInformation($"{nameof(MoveAsync)}: s3Key = {s3key}");

            var result = new List<ResponseUploadDto>();
            try
            {
                foreach (var item in s3key)
                {
                    var newS3Key = GenerateNewMoveS3Key(item);
                    var contentType = string.Empty;

                    // Copy the object from the source to the destination
                    CopyObjectRequest copyRequest =
                        new()
                        {
                            SourceBucket = BucketName,
                            SourceKey = item,
                            DestinationBucket = BucketName,
                            DestinationKey = newS3Key
                        };

                    CopyObjectResponse copyResponse = await _s3Client.CopyObjectAsync(copyRequest);

                    // Delete the original object
                    DeleteObjectRequest deleteRequest =
                        new() { BucketName = BucketName, Key = item };

                    DeleteObjectResponse deleteResponse = await _s3Client.DeleteObjectAsync(
                        deleteRequest
                    );
                    result.Add(
                        MapResponse(
                            item,
                            $"{BucketName}/{newS3Key}",
                            contentType,
                            GetFileNameFromPath(newS3Key)
                        )
                    );
                }
            }
            catch (AmazonS3Exception e)
            {
                _logger.LogError($"{MoveAsync}: Lỗi S3 khi move file: {e.Message}");
                throw new S3BucketException(S3ManagerFileErrorCode.MoveFileError);
            }
            catch (Exception e)
            {
                _logger.LogError($"{MoveAsync}: Lỗi khi move file: {e.Message}");
                throw new S3BucketException(S3ManagerFileErrorCode.MoveFileBadRequest);
            }
            return result;
        }

        public async Task<Stream> ReadAsync(string path)
        {
            _logger.LogInformation($"{nameof(ReadAsync)}: path = {path}");
            using HttpClient httpClient = new HttpClient();
            var uri = new Uri($"{_config.ViewMediaUrl}/{path}");
            var response = await httpClient.GetAsync(uri);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var errorMessage = await response.Content.ReadFromJsonAsync<ResponseContentDto>();
                //Xử lý các ngoại lệ lúc đọc file từ server
                _logger.LogError(
                    $"{nameof(ReadAsync)}: responseBody = {await response.Content.ReadAsStringAsync()}, responseStatusCode = {response.StatusCode}"
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

        public async Task<List<ResponseUploadDto>> UploadAsync(params IFormFile[] input)
        {
            _logger.LogInformation($"{nameof(UploadAsync)}:");

            var result = new List<ResponseUploadDto>();
            foreach (var item in input)
            {
                try
                {
                    string fileName = item.FileName;
                    var contentType = item.ContentType;

                    var extension = Path.GetExtension(fileName);

                    // Loại bỏ phần mở rộng của file
                    string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

                    fileName = StringUtils
                        .RemoveDiacritics(nameWithoutExtension)
                        .ToLower()
                        .Replace(" ", string.Empty);

                    string s3Key =
                        $"temp/{GenerateObjectName()}/{fileName}-{DateTime.Now.ToFileTime()}";

                    await S3BucketExistAsync(BucketName);
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        if (
                            FileExtensions.ImageExtensions.Contains(extension)
                            && ImageUtils.TryResizeImage(item)
                        )
                        {
                            s3Key = $"{s3Key}{FileTypes.WEBP}";
                        }
                        else
                        {
                            s3Key = $"{s3Key}{Path.GetExtension(item.FileName)}";
                        }

                        await item.CopyToAsync(memoryStream);
                        memoryStream.Position = 0;
                        var putRequest = new PutObjectRequest
                        {
                            BucketName = BucketName,
                            Key = s3Key,
                            InputStream = memoryStream,
                            ContentType = contentType
                        };
                        var response = await _s3Client.PutObjectAsync(putRequest);
                    }
                    result.Add(MapResponse(s3Key, s3Key, contentType, GetFileNameFromPath(s3Key)));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"{UploadAsync}: Lỗi khi upload file: {ex.Message}");
                    throw new S3BucketException(S3ManagerFileErrorCode.UploadMediaError);
                }
            }
            return result;
        }

        public async Task<List<ResponseUploadDto>> UploadFileAsync(params IFormFile[] input)
        {
            _logger.LogInformation($"{nameof(UploadFileAsync)}:");

            var result = new List<ResponseUploadDto>();
            foreach (var item in input)
            {
                try
                {
                    string fileName = item.FileName;
                    var contentType = item.ContentType;

                    var extension = Path.GetExtension(fileName);

                    // Loại bỏ phần mở rộng của file
                    string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

                    fileName = StringUtils
                        .RemoveDiacritics(nameWithoutExtension)
                        .ToLower()
                        .Replace(" ", string.Empty);

                    string s3Key = $"{GenerateObjectName()}/{fileName}-{DateTime.Now.ToFileTime()}";

                    await S3BucketExistAsync(BucketName);
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        if (
                            FileExtensions.ImageExtensions.Contains(extension)
                            && ImageUtils.TryResizeImage(item)
                        )
                        {
                            s3Key = $"{s3Key}{FileTypes.WEBP}";
                        }
                        else
                        {
                            s3Key = $"{s3Key}{Path.GetExtension(item.FileName)}";
                        }

                        await item.CopyToAsync(memoryStream);
                        memoryStream.Position = 0;
                        var putRequest = new PutObjectRequest
                        {
                            BucketName = BucketName,
                            Key = s3Key,
                            InputStream = memoryStream,
                            ContentType = contentType,
                        };
                        var response = await _s3Client.PutObjectAsync(putRequest);
                    }
                    result.Add(MapResponse(s3Key, s3Key, contentType, GetFileNameFromPath(s3Key)));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"{UploadAsync}: Lỗi khi upload file: {ex.Message}");
                    throw new S3BucketException(S3ManagerFileErrorCode.UploadMediaError);
                }
            }
            return result;
        }
    }
}
