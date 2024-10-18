using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using CR.FptEkyc.Configs;
using CR.FptEkyc.Constants;
using CR.FptEkyc.Dtos;
using CR.FptEkyc.Exceptions;
using CR.InfrastructureBase.Ekyc.Dtos;
using CR.Utils.DataUtils;
using CR.Utils.Net.File;
using CR.Utils.Net.MimeTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace CR.FptEkyc
{
    public class FptPocService : FptEkycService, IFptPocService
    {
        private readonly FptPocConfig _config;

        public FptPocService(
            ILogger<FptEkycService> logger,
            IHttpContextAccessor httpContext,
            IOptions<FptPocConfig> config
        )
            : base(logger, httpContext)
        {
            _config = config.Value;
            _httpClient = InitHttpClient();
        }

        private HttpClient InitHttpClient()
        {
            HttpClient httpClient = new() { BaseAddress = new Uri(_config.BaseUrl) };

            httpClient.DefaultRequestHeaders.Add("token", _config.Token);
            httpClient.DefaultRequestHeaders.Add("code", _config.Code);
            if (
                _httpContext
                    .HttpContext?.Request
                    .Headers.Any(x => x.Key == FptPocConfig.CodeTransaction) == true
            )
            {
                httpClient.DefaultRequestHeaders.Add(
                    FptPocConfig.CodeTransaction,
                    _httpContext
                        .HttpContext?.Request
                        .Headers.FirstOrDefault(x => x.Key == FptPocConfig.CodeTransaction)
                        .Value.ToString()
                );
            }
            return httpClient;
        }

        public override async Task<FaceMatchResultDto> FaceMatch(EkycFaceMatchDto input)
        {
            _logger.LogInformation(
                $"{nameof(FaceMatch)}: input = {JsonSerializer.Serialize(input)}"
            );
            OcrPocFaceMatchRequestDto requestBody =
                new()
                {
                    AnhMatTruoc = ImageUtils.ConvertImageToBase64(
                        ImageUtils.ConvertIFormFileToImage(input.FrontIdImage),
                        JpegFormat.Instance
                    ),
                    AnhKhachHang = ImageUtils.ConvertImageToBase64(
                        ImageUtils.ConvertIFormFileToImage(input.FaceImage),
                        JpegFormat.Instance
                    ),
                };

            StringContent content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                MimeTypeNames.ApplicationJson
            );

            HttpResponseMessage resOCRApi = await _httpClient.PostAsync(
                FptPocConfig.ApiFaceMatch,
                content
            );

            string resContentString = await resOCRApi.Content.ReadAsStringAsync();

            switch (resOCRApi.StatusCode)
            {
                case HttpStatusCode.OK:
                    break;
                case HttpStatusCode.Forbidden:
                    _logger.LogError(
                        $"Call Api Face Match fail with status = {resOCRApi.StatusCode}, response = {resContentString}"
                    );
                    throw new FptEkycException(FptEkycErrorCode.OCRNotPermission);
                case HttpStatusCode.BadRequest:
                    _logger.LogError(
                        $"Call Api Face Match fail with status = {resOCRApi.StatusCode}, response = {resContentString}"
                    );
                    throw new FptEkycException(FptEkycErrorCode.OCRErrorFromFPTPoc);
                default:
                    _logger.LogError(
                        $"Call Api Face Match fail with status = {resOCRApi.StatusCode}, response = {resContentString}"
                    );
                    throw new FptEkycException(FptEkycErrorCode.PersonalCustomerHttpError);
            }
            OcrPocResponseDto responseData = JsonSerializer.Deserialize<OcrPocResponseDto>(
                resContentString
            )!;
            FaceMatchResultDto result;
            if (responseData.Status != OcrErrorCodes.NO_ERROR_OTHER)
            {
                _logger.LogError(
                    $"Call Api Face Match fail with status = {responseData.Status}, response = {resContentString}"
                );
                throw new FptEkycException(
                    FptEkycErrorCode.OCRErrorFromFPTPoc,
                    responseData.CodeMessage == null
                        ? responseData.Message
                        : $"{responseData.Message} ({responseData.CodeMessage})"
                );
            }
            else
            {
                result = new FaceMatchResultDto() { IsMatch = true, };
            }
            return result;
        }

        public override async Task<FaceMatchResultDto> Liveness(EkycLivenessDto input)
        {
            _logger.LogInformation(
                $"{nameof(Liveness)}: input = {JsonSerializer.Serialize(input)}"
            );
            OcrPocLivenessRequestDto requestBody =
                new()
                {
                    AnhMatTruoc = ImageUtils.ConvertImageToBase64(
                        ImageUtils.ConvertIFormFileToImage(input.IdFrontImage),
                        JpegFormat.Instance
                    ),
                    AnhVideo = new(),
                };
            foreach (var item in input.ImageVideo)
            {
                requestBody.AnhVideo.Add(
                    new OcrPocLivenessImageVideoRequestDto
                    {
                        Anh = ImageUtils.ConvertImageToBase64(
                            ImageUtils.ConvertIFormFileToImage(item.Image),
                            JpegFormat.Instance
                        ),
                        ThoiGian = item.Time
                    }
                );
            }
            StringContent content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                MimeTypeNames.ApplicationJson
            );

            if (input.CodeTransaction != null)
            {
                _httpClient.DefaultRequestHeaders.Add(
                    FptPocConfig.CodeTransaction,
                    input.CodeTransaction
                );
            }

            HttpResponseMessage resApiLiveness = await _httpClient.PostAsync(
                FptPocConfig.ApiLiveness,
                content
            );

            string resContentString = await resApiLiveness.Content.ReadAsStringAsync();

            switch (resApiLiveness.StatusCode)
            {
                case HttpStatusCode.OK:
                    break;
                case HttpStatusCode.Forbidden:
                    _logger.LogError(
                        $"{nameof(Liveness)}: Call Api Liveness fail with status = {resApiLiveness.StatusCode}, response = {resContentString}"
                    );
                    throw new FptEkycException(FptEkycErrorCode.OCRNotPermission);
                case HttpStatusCode.BadRequest:
                    _logger.LogError(
                        $"{nameof(Liveness)}: Call Api Liveness fail with status = {resApiLiveness.StatusCode}, response = {resContentString}"
                    );
                    throw new FptEkycException(FptEkycErrorCode.OCRErrorFromFPTPoc);
                default:
                    _logger.LogError(
                        $"{nameof(Liveness)}: Call Api Liveness fail with status = {resApiLiveness.StatusCode}, response = {resContentString}"
                    );
                    throw new FptEkycException(FptEkycErrorCode.PersonalCustomerHttpError);
            }
            OcrPocLivenessResponseDto responseData =
                JsonSerializer.Deserialize<OcrPocLivenessResponseDto>(resContentString)!;
            FaceMatchResultDto result;
            if (responseData.Status != OcrErrorCodes.NO_ERROR_OTHER)
            {
                _logger.LogError(
                    $"{nameof(Liveness)}: Call Api Liveness fail with status = {responseData.Status}, response = {resContentString}"
                );
                throw new FptEkycException(
                    FptEkycErrorCode.OCRErrorFromFPTPoc,
                    responseData.CodeMessage == null
                        ? responseData.Message
                        : $"{responseData.Message} ({responseData.CodeMessage})"
                );
            }
            else
            {
                result = new FaceMatchResultDto()
                {
                    IsMatch = true,
                    Similarity = responseData.Data ?? 0
                };
            }
            return result;
        }

        public override async Task<OcrIdResultDto> OcrIdCard(
            bool isPassport,
            IFormFile frontIdImage,
            IFormFile? backIdImage,
            string type,
            string? codeTransaction = null
        )
        {
            _logger.LogInformation(
                $"{nameof(OcrIdCard)}: isPassport = {isPassport}, frontIdImage = {frontIdImage?.FileName} {frontIdImage?.Length},"
                    + $" backIdImage = {backIdImage?.FileName} {backIdImage?.Length} byte"
            );

            OcrIdResultDto resultOcr = new();
            string nationality = "Việt Nam";
            if (codeTransaction != null)
            {
                _httpClient.DefaultRequestHeaders.Add(
                    FptPocConfig.CodeTransaction,
                    codeTransaction
                );
            }
            if (isPassport && frontIdImage != null)
            {
                OcrPassportData passportData = await ReadPassport(frontIdImage);
                resultOcr = new OcrIdResultDto
                {
                    IdNo = RecognitionUtils.GetValueStandard(passportData.SoCmt ?? ""),
                    Name = RecognitionUtils.GetValueStandard(passportData.HoVaTen ?? ""),
                    Sex = OcrGenders.ConvertStandard(passportData.GioiTinh ?? ""),
                    DateOfBirth = DateTimeUtils.FromDateStrDD_MM_YYYY_ToDate(
                        passportData.NamSinh ?? ""
                    ),
                    PlaceOfOrigin = RecognitionUtils.GetValueStandard(passportData.DiaChi ?? ""),
                    PassportIdNumber = RecognitionUtils.GetValueStandard(
                        passportData.SoHoChieu ?? ""
                    ),
                    IdIssueDate = DateTimeUtils.FromDateStrDD_MM_YYYY_ToDate(
                        passportData.NgayCap ?? ""
                    ),
                    IdIssueExpDate = DateTimeUtils.FromDateStrDD_MM_YYYY_ToDate(
                        passportData.NgayHetHan ?? ""
                    ),
                    Nationality = nationality,
                    IdIssuer = passportData.NoiCap,
                };
                CheckAge(resultOcr.DateOfBirth);
                CheckExp(resultOcr.IdIssueExpDate);
            }
            else if (!isPassport && frontIdImage != null && backIdImage != null)
            {
                OcrData responseData = await ReadIdentification(frontIdImage, backIdImage, type);

                CheckUploadTypeAndIdType(
                    type,
                    responseData.LoaiCmtMatTruoc ?? responseData.LoaiCmtKhacMatTruoc ?? ""
                );

                var issueDate = ConvertStringIssudeDateToDateTime(responseData.NgayCap ?? "");
                resultOcr = new OcrIdResultDto
                {
                    IdNo = RecognitionUtils.GetValueStandard(responseData.SoCmt ?? ""),
                    Name = RecognitionUtils.GetValueStandard(responseData.HoVaTen ?? ""),
                    Sex = OcrGenders.ConvertStandard(
                        RecognitionUtils.GetValueStandard(responseData.GioiTinh ?? "")
                    ),
                    DateOfBirth = DateTimeUtils.FromDateStrDD_MM_YYYY_ToDate(
                        responseData.NamSinh ?? ""
                    ),
                    IdIssueDate = issueDate,
                    IdIssueExpDate = ProccessExpDate(
                        responseData.NgayHetHan ?? "",
                        issueDate,
                        type.ToUpper()
                    ),
                    IdIssuer = RecognitionUtils.GetValueStandard(responseData.NoiCap ?? ""),
                    PlaceOfOrigin = RecognitionUtils.GetValueStandard(responseData.QueQuan ?? ""),
                    PlaceOfResidence = RecognitionUtils.GetValueStandard(responseData.NoiTru ?? ""),
                    Nationality = nationality,
                };
            }
            return resultOcr;
        }

        protected async Task<OcrData> ReadIdentification(
            IFormFile frontImage,
            IFormFile backImage,
            string type
        )
        {
            var frontImageBase64 = ImageUtils.ConvertImageToBase64(
                ImageUtils.ConvertIFormFileToImage(frontImage),
                JpegFormat.Instance
            );
            var backImageBase64 = ImageUtils.ConvertImageToBase64(
                ImageUtils.ConvertIFormFileToImage(backImage),
                JpegFormat.Instance
            );
            OcrPocRequestDto requestBody =
                new()
                {
                    AnhMatTruoc = frontImageBase64,
                    AnhMatSau = backImageBase64,
                    MaGiayTo = FptPocCardTypes.GetPocType(type),
                };

            StringContent content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                MimeTypeNames.ApplicationJson
            );

            HttpResponseMessage resOCRApi = await _httpClient.PostAsync(
                FptPocConfig.ApiOcrId,
                content
            );

            string resContentString = await resOCRApi.Content.ReadAsStringAsync();

            switch (resOCRApi.StatusCode)
            {
                case HttpStatusCode.OK:
                    break;
                case HttpStatusCode.Forbidden:
                    _logger.LogError(
                        $"Call Api OCR fail Identification Image with status = {resOCRApi.StatusCode}, response = {resContentString}"
                    );
                    throw new FptEkycException(FptEkycErrorCode.OCRNotPermission);
                case HttpStatusCode.BadRequest:
                    _logger.LogError(
                        $"Call Api OCR fail Identification Image with status = {resOCRApi.StatusCode}, response = {resContentString}"
                    );
                    throw new FptEkycException(FptEkycErrorCode.PersonalCustomerOCRPassport);
                default:
                    _logger.LogError(
                        $"Call Api OCR fail Identification Image with status = {resOCRApi.StatusCode}, response = {resContentString}"
                    );
                    throw new FptEkycException(FptEkycErrorCode.PersonalCustomerHttpError);
            }
            OcrPocIdenResponseDto responseData = JsonSerializer.Deserialize<OcrPocIdenResponseDto>(
                resContentString
            )!;
            OcrData identificationData;
            if (responseData.Status != OcrErrorCodes.NO_ERROR_OTHER)
            {
                _logger.LogError(
                    $"Call Api OCR fail Passport Image with status = {responseData.Status}, response = {resContentString}"
                );
                throw new FptEkycException(
                    FptEkycErrorCode.OCRErrorFromFPTPoc,
                    responseData.CodeMessage == null
                        ? responseData.Message
                        : $"{responseData.Message} ({responseData.CodeMessage})"
                );
            }
            else
            {
                identificationData = responseData.Data ?? new();
            }
            return identificationData;
        }

        public async Task<OcrPocCodeTransactionResponseDto> GetCodeTransaction()
        {
            _logger.LogInformation($"{nameof(GetCodeTransaction)}");
            HttpResponseMessage resApiLiveness = await _httpClient.GetAsync(
                FptPocConfig.ApiCodeTransaction
            );

            string resContentString = await resApiLiveness.Content.ReadAsStringAsync();

            switch (resApiLiveness.StatusCode)
            {
                case HttpStatusCode.OK:
                    break;
                case HttpStatusCode.Forbidden:
                    _logger.LogError(
                        $"{nameof(GetCodeTransaction)}: Call Api fail with status = {resApiLiveness.StatusCode}, response = {resContentString}"
                    );
                    throw new FptEkycException(FptEkycErrorCode.OCRNotPermission);
                case HttpStatusCode.BadRequest:
                    _logger.LogError(
                        $"{nameof(GetCodeTransaction)}: Call Api fail with status = {resApiLiveness.StatusCode}, response = {resContentString}"
                    );
                    throw new FptEkycException(FptEkycErrorCode.OCRErrorFromFPTPoc);
                default:
                    _logger.LogError(
                        $"{nameof(GetCodeTransaction)}: Call Api fail with status = {resApiLiveness.StatusCode}, response = {resContentString}"
                    );
                    throw new FptEkycException(FptEkycErrorCode.PersonalCustomerHttpError);
            }
            OcrPocCodeTransactionResponseDto responseData =
                JsonSerializer.Deserialize<OcrPocCodeTransactionResponseDto>(resContentString)!;
            if (responseData.Status != OcrErrorCodes.NO_ERROR_OTHER)
            {
                throw new FptEkycException(
                    FptEkycErrorCode.OCRErrorFromFPTPoc,
                    responseData.CodeMessage == null
                        ? responseData.Message
                        : $"{responseData.Message} ({responseData.CodeMessage})"
                );
            }
            return responseData;
        }

        protected new async Task<OcrPassportData> ReadPassport(IFormFile passportImage)
        {
            var imageBase64 = ImageUtils.ConvertImageToBase64(
                ImageUtils.ConvertIFormFileToImage(passportImage),
                JpegFormat.Instance
            );
            OcrPocRequestDto requestBody =
                new() { AnhMatTruoc = imageBase64, MaGiayTo = FptPocCardTypes.PASSPORT };

            StringContent content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                MimeTypeNames.ApplicationJson
            );

            HttpResponseMessage resOCRApi = await _httpClient.PostAsync(
                FptPocConfig.ApiOcrId,
                content
            );

            string resContentString = await resOCRApi.Content.ReadAsStringAsync();
            switch (resOCRApi.StatusCode)
            {
                case HttpStatusCode.OK:
                    break;
                case HttpStatusCode.Forbidden:
                    _logger.LogError(
                        $"Call Api OCR fail Passport Image with status = {resOCRApi.StatusCode}, response = {resContentString}"
                    );
                    throw new FptEkycException(FptEkycErrorCode.OCRNotPermission);
                case HttpStatusCode.BadRequest:
                    _logger.LogError(
                        $"Call Api OCR fail Passport Image with status = {resOCRApi.StatusCode}, response = {resContentString}"
                    );
                    throw new FptEkycException(FptEkycErrorCode.PersonalCustomerOCRPassport);
                default:
                    _logger.LogError(
                        $"Call Api OCR fail Passport Image with status = {resOCRApi.StatusCode}, response = {resContentString}"
                    );
                    throw new FptEkycException(FptEkycErrorCode.PersonalCustomerHttpError);
            }
            OcrPocPassportResponseDto responseData =
                JsonSerializer.Deserialize<OcrPocPassportResponseDto>(resContentString)!;
            OcrPassportData passportData;
            if (responseData.Status != OcrErrorCodes.NO_ERROR_OTHER)
            {
                _logger.LogError(
                    $"Call Api OCR fail Passport Image with status = {responseData.Status}, response = {resContentString}"
                );
                throw new FptEkycException(
                    FptEkycErrorCode.OCRErrorFromFPTPoc,
                    responseData.CodeMessage == null
                        ? responseData.Message
                        : $"{responseData.Message} ({responseData.CodeMessage})"
                );
            }
            else
            {
                passportData = responseData.Data ?? new();
            }
            return passportData;
        }

        /// <summary>
        /// Kiểm tra loại giấy tờ người dùng chọn và loại trên giấy tờ có khớp nhau ko
        /// </summary>
        public virtual void CheckUploadTypeAndIdType(string uploadType, string type)
        {
            bool cmndOk =
                uploadType == CardTypesInput.CMND
                && new string[] { FptPocTypes.CMT_09_FRONT, FptPocTypes.CMT_12_FRONT }.Contains(
                    type
                );
            bool cccdOk =
                uploadType == CardTypesInput.CCCD
                && new string[] { FptPocTypes.CCCD_12_FRONT, FptPocTypes.CCCD_CMT_FRONT }.Contains(
                    type
                );
            bool chipOk =
                uploadType == CardTypesInput.CCCD_CHIP && type == FptPocTypes.CCCD_CHIP_FRONT;

            if (!(cmndOk || cccdOk || chipOk))
            {
                throw new FptEkycException(FptEkycErrorCode.PersonalCustomerOCRIDTypeInvalid);
            }
        }
    }
}
