using System.Net;
using System.Net.Http;
using System.Text.Json;
using CR.ApplicationBase;
using CR.Authentication.Infrastructure.Persistence;
using CR.Constants.CustomerIdentification;
using CR.Constants.SysVar;
using CR.FptEkyc.Configs;
using CR.FptEkyc.Constants;
using CR.FptEkyc.Dtos;
using CR.FptEkyc.Exceptions;
using CR.InfrastructureBase.Ekyc;
using CR.InfrastructureBase.Ekyc.Dtos;
using CR.Utils.DataUtils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;

namespace CR.FptEkyc
{
    public class FptEkycService : ServiceBase<AuthenticationDbContext>, IEkyc
    {
        private readonly FptEkycConfig? _config;
        protected HttpClient _httpClient;

        public FptEkycService(ILogger logger, IHttpContextAccessor httpContext)
            : base(logger, httpContext)
        {
            _httpClient = new();
        }

        public FptEkycService(
            ILogger<FptEkycService> logger,
            IHttpContextAccessor httpContext,
            IOptions<FptEkycConfig> config
        )
            : base(logger, httpContext)
        {
            _config = config.Value;
            _httpClient = InitHttpClient();
        }

        private HttpClient InitHttpClient()
        {
            HttpClient httpClient = new() { BaseAddress = new Uri(FptEkycConfig.ApiBaseAddress) };

            httpClient.DefaultRequestHeaders.Add(
                "api-key",
                _dbContext
                    .SysVars.Where(o => o.GrName == GrNames.EKYC && o.VarName == VarNames.API_KEY)
                    .Select(o => o.VarValue)
            );
            return httpClient;
        }

        /// <summary>
        /// Kiểm tra độ khớp khuôn mặt
        /// </summary>
        /// <param name="similarity"></param>
        /// <returns></returns>
        private bool CheckFaceSimilarity(double similarity)
        {
            var similarityFind = _dbContext.SysVars.FirstOrDefault(o =>
                o.GrName == GrNames.EKYC && o.VarName == VarNames.FACE_SIMILARITY
            );
            if (similarityFind == null)
            {
                return false;
            }
            return similarity >= double.Parse(similarityFind.VarValue);
        }

        public virtual async Task<FaceMatchResultDto> FaceMatch(EkycFaceMatchDto input)
        {
            _logger.LogInformation(
                $"{nameof(FaceMatch)}: frontIdImage = {input.FrontIdImage?.FileName} {input.FrontIdImage?.Length},"
                    + $" faceImage = {input.FaceImage?.FileName} {input.FaceImage?.Length} byte"
            );

            FaceMatchData faceMatchData = await FaceRecognition(
                input.FrontIdImage!,
                input.FaceImage!
            );

            if (CheckFaceSimilarity(faceMatchData.Similarity))
            {
                //string faceImageUrl = _imageServices.UploadFileID(new ImageAPI.Models.UploadFileModel
                //{
                //    File = input.FaceImage,
                //    Folder = FileFolder.INVESTOR,
                //});

                //_investorRepository.FinishEKYC(input.Phone, faceImageUrl);
            }
            else
            {
                throw new FptEkycException(
                    FptEkycErrorCode.PersonalCustomerFaceRecognitionNotMatch
                );
            }

            return new FaceMatchResultDto
            {
                IsMatch = faceMatchData.IsMatch,
                Similarity = faceMatchData.Similarity,
                IsBothImgIDCard = faceMatchData.IsBothImgIDCard
            };
        }

        public virtual async Task<OcrIdResultDto> OcrIdCard(
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

            if (isPassport && frontIdImage != null)
            {
                OcrDataPassport passportData = await ReadPassport(frontIdImage);

                resultOcr = new OcrIdResultDto
                {
                    IdNo = RecognitionUtils.GetValueStandard(passportData.PassportNumber ?? ""),
                    Name = RecognitionUtils.GetValueStandard(passportData.Name ?? ""),
                    Sex = OcrGenders.ConvertStandard(passportData.Sex ?? ""),
                    DateOfBirth = DateTimeUtils.FromDateStrDD_MM_YYYY_ToDate(
                        passportData.Dob ?? ""
                    ),
                    PlaceOfOrigin = RecognitionUtils.GetValueStandard(passportData.Pob ?? ""),
                    PassportIdNumber = RecognitionUtils.GetValueStandard(
                        passportData.PassportNumber ?? ""
                    ),
                    IdIssueDate = DateTimeUtils.FromDateStrDD_MM_YYYY_ToDate(
                        passportData.Doi ?? ""
                    ),
                    IdIssueExpDate = DateTimeUtils.FromDateStrDD_MM_YYYY_ToDate(
                        passportData.Doe ?? ""
                    ),
                    Nationality = nationality,
                    IdIssuer = passportData.IdIssuer,
                };

                CheckAge(resultOcr.DateOfBirth);
                CheckExp(resultOcr.IdIssueExpDate);
            }
            else if (!isPassport && frontIdImage != null && backIdImage != null)
            {
                //front id
                OcrFrontIdDataNewType frontData = await ReadFrontIdDataNewType(frontIdImage);
                CheckUploadTypeAndIdType(type, frontData);

                //back id
                OcrBackIdDataNewType backData = await ReadBackIdDataNewType(backIdImage);
                CheckDifferenceImage(frontData, backData);

                var issueDate = ConvertStringIssudeDateToDateTime(backData.IssueDate ?? "");

                resultOcr = new OcrIdResultDto
                {
                    IdNo = RecognitionUtils.GetValueStandard(frontData.Id ?? ""),
                    Name = RecognitionUtils.GetValueStandard(frontData.Name ?? ""),
                    Sex = OcrGenders.ConvertStandard(
                        RecognitionUtils.GetValueStandard(frontData.Sex ?? "")
                    ),
                    DateOfBirth = DateTimeUtils.FromDateStrDD_MM_YYYY_ToDate(frontData.Dob ?? ""),
                    IdIssueDate = issueDate,
                    IdIssueExpDate = ProccessExpDate(
                        frontData.Doe ?? "",
                        issueDate,
                        type.ToUpper()
                    ),
                    IdIssuer = RecognitionUtils.GetValueStandard(backData.IssueLoc ?? ""),
                    PlaceOfOrigin = RecognitionUtils.GetValueStandard(frontData.Home ?? ""),
                    PlaceOfResidence = RecognitionUtils.GetValueStandard(frontData.Address ?? ""),
                    Nationality = nationality
                };
            }
            return resultOcr;
        }

        public void CheckIdType(string typeRequest, string typeResponse)
        {
            if (typeRequest != typeResponse)
            {
                throw new FptEkycException(FptEkycErrorCode.PersonalCustomerOCRIDTypeInvalid);
            }
        }

        public void CheckBackImage(IFormFile bankImage)
        {
            if (bankImage == null)
            {
                throw new FptEkycException(FptEkycErrorCode.PersonalCustomerOCRBackIdNotEmpty);
            }
        }

        /// <summary>
        /// Kiểm tra ảnh khác loại của cmnd/cccd
        /// </summary>
        /// <param name="frontData"></param>
        /// <param name="backData"></param>
        public virtual void CheckDifferenceImage(
            OcrFrontIdDataNewType frontData,
            OcrBackIdDataNewType backData
        )
        {
            bool cmndOk =
                frontData.Type == OcrTypes.CMND_FRONT && backData.Type == OcrTypes.CMND_BACK;
            bool cccdOk =
                frontData.Type == OcrTypes.CCCD_FRONT && backData.Type == OcrTypes.CCCD_BACK;
            bool chipOk =
                frontData.Type == OcrTypes.CCCD_CHIP_FRONT
                && backData.Type == OcrTypes.CCCD_CHIP_BACK;

            if (!(cmndOk || cccdOk || chipOk))
            {
                throw new FptEkycException(FptEkycErrorCode.PersonalCustomerOCRDifference);
            }
        }

        /// <summary>
        /// Kiểm tra loại giấy tờ người dùng chọn và loại trên giấy tờ có khớp nhau ko
        /// </summary>
        /// <param name="uploadType"></param>
        /// <param name="frontData"></param>
        public virtual void CheckUploadTypeAndIdType(
            string uploadType,
            OcrFrontIdDataNewType frontData
        )
        {
            var listCmndTypeOcr = new string[]
            {
                OcrTypesNews.CMND_12_FRONT,
                OcrTypesNews.CMND_09_FRONT
            };
            bool cmndOk =
                uploadType == CardTypesInput.CMND && listCmndTypeOcr.Contains(frontData.TypeNew);
            bool cccdOk =
                uploadType == CardTypesInput.CCCD
                && frontData.TypeNew == OcrTypesNews.CCCD_12_FRONT;
            bool chipOk =
                uploadType == CardTypesInput.CCCD_CHIP
                && frontData.TypeNew == OcrTypesNews.CCCD_CHIP_FRONT;

            if (!(cmndOk || cccdOk || chipOk))
            {
                throw new FptEkycException(FptEkycErrorCode.PersonalCustomerOCRIDTypeInvalid);
            }
        }

        /// <summary>
        /// Đọc cmnd/cccd mặt trước
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        protected async Task<OcrFrontIdDataNewType> ReadFrontIdDataNewType(IFormFile image)
        {
            MultipartFormDataContent formDataContent = new();
            MemoryStream memoryStream = new();

            image.CopyTo(memoryStream);
            formDataContent.Add(
                new ByteArrayContent(memoryStream.ToArray()),
                "image",
                image.FileName
            );

            HttpResponseMessage resOCRApi = await _httpClient.PostAsync(
                FptEkycConfig.ApiOCRId,
                formDataContent
            );
            string resContentString = await resOCRApi.Content.ReadAsStringAsync();
            switch (resOCRApi.StatusCode)
            {
                case HttpStatusCode.OK:
                    break;
                case HttpStatusCode.BadRequest:
                    _logger.LogError(
                        $"Call Api OCR fail Front ID Image with status = {resOCRApi.StatusCode}, response = {resContentString}"
                    );
                    throw new FptEkycException(FptEkycErrorCode.PersonalCustomerOCRFrontId);

                default:
                    _logger.LogError(
                        $"Call Api OCR fail Front ID Image with status = {resOCRApi.StatusCode}, response = {resContentString}"
                    );
                    throw new FptEkycException(FptEkycErrorCode.PersonalCustomerHttpError);
            }
            OcrResponseFrontIdNewType OCRResFrontImage =
                JsonSerializer.Deserialize<OcrResponseFrontIdNewType>(resContentString)!;
            OcrFrontIdDataNewType result = new();
            if (OCRResFrontImage.ErrorCode != OcrErrorCodes.NO_ERROR)
            {
                _logger.LogError(
                    $"Call Api OCR fail Front ID Image with status = {resOCRApi.StatusCode}, response = {resContentString}"
                );
                throw new FptEkycException(FptEkycErrorCode.PersonalCustomerOCRFrontId);
            }
            else
            {
                _logger.LogInformation($"OCR success with response = {resContentString}");
                result = OCRResFrontImage.Data?.FirstOrDefault() ?? new();
            }

            string nationality = "Việt Nam";
            if (string.IsNullOrEmpty(result.Nationality))
            {
                nationality = RecognitionUtils.GetValueStandard(result.Nationality ?? "");
            }
            result.Nationality = nationality;
            return result;
        }

        /// <summary>
        /// Đọc cmnd/cccd mặt sau
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        protected async Task<OcrBackIdDataNewType> ReadBackIdDataNewType(IFormFile image)
        {
            MultipartFormDataContent formDataContent = new();
            MemoryStream memoryStream = new();

            image.CopyTo(memoryStream);
            formDataContent.Add(
                new ByteArrayContent(memoryStream.ToArray()),
                "image",
                image.FileName
            );

            HttpResponseMessage resOCRApi = await _httpClient.PostAsync(
                FptEkycConfig.ApiOCRId,
                formDataContent
            );
            string resContentString = await resOCRApi.Content.ReadAsStringAsync();
            switch (resOCRApi.StatusCode)
            {
                case HttpStatusCode.OK:
                    break;
                case HttpStatusCode.BadRequest:
                    _logger.LogError(
                        $"Call Api OCR fail Back ID Image with status = {resOCRApi.StatusCode}, response = {resContentString}"
                    );
                    throw new FptEkycException(FptEkycErrorCode.PersonalCustomerOCRBackId);
                default:
                    _logger.LogError(
                        $"Call Api OCR fail Back ID Image with status = {resOCRApi.StatusCode}, response = {resContentString}"
                    );
                    throw new FptEkycException(FptEkycErrorCode.PersonalCustomerHttpError);
            }

            OcrResponseBackIdNewType OCRResBackImage =
                JsonSerializer.Deserialize<OcrResponseBackIdNewType>(resContentString)!;
            OcrBackIdDataNewType result;
            if (OCRResBackImage.ErrorCode != OcrErrorCodes.NO_ERROR)
            {
                _logger.LogError(
                    $"Call Api OCR fail Back ID Image with status = {resOCRApi.StatusCode}, response = {resContentString}"
                );
                throw new FptEkycException(FptEkycErrorCode.PersonalCustomerOCRBackId);
            }
            else
            {
                _logger.LogInformation($"OCR Back ID success with response = {resContentString}");
                result = OCRResBackImage.Data?.FirstOrDefault() ?? new();
            }

            return result;
        }

        /// <summary>
        /// Đọc hộ chiếu
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        protected async Task<OcrDataPassport> ReadPassport(IFormFile image)
        {
            MultipartFormDataContent formDataContent = new();
            MemoryStream memoryStream = new();

            image.CopyTo(memoryStream);
            formDataContent.Add(
                new ByteArrayContent(memoryStream.ToArray()),
                "image",
                image.FileName
            );

            HttpResponseMessage resOCRApi = await _httpClient.PostAsync(
                FptEkycConfig.ApiOCRPassport,
                formDataContent
            );
            string resContentString = await resOCRApi.Content.ReadAsStringAsync();
            switch (resOCRApi.StatusCode)
            {
                case HttpStatusCode.OK:
                    break;
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
            OcrResponsePassport OCRResPassport = JsonSerializer.Deserialize<OcrResponsePassport>(
                resContentString
            )!;
            OcrDataPassport passportData;
            if (OCRResPassport.ErrorCode != OcrErrorCodes.NO_ERROR)
            {
                _logger.LogError(
                    $"Call Api OCR fail Passport Image with status = {resOCRApi.StatusCode}, response = {resContentString}"
                );
                throw new FptEkycException(FptEkycErrorCode.PersonalCustomerOCRPassport);
            }
            else
            {
                _logger.LogInformation($"OCR Passport success with response = {resContentString}");
                passportData = OCRResPassport.Data?.FirstOrDefault() ?? new();
            }

            return passportData;
        }

        /// <summary>
        /// So khớp khuôn mặt
        /// </summary>
        /// <param name="idImageUrl"></param>
        /// <param name="faceImageUrl"></param>
        /// <returns></returns>
        protected async Task<FaceMatchData> FaceRecognition(
            IFormFile idImageUrl,
            IFormFile faceImageUrl
        )
        {
            MultipartFormDataContent formDataContent = new();

            MemoryStream memoryStream = new();
            idImageUrl.CopyTo(memoryStream);
            formDataContent.Add(
                new ByteArrayContent(memoryStream.ToArray()),
                "file[]",
                idImageUrl.FileName
            );

            memoryStream = new();
            faceImageUrl.CopyTo(memoryStream);
            formDataContent.Add(
                new ByteArrayContent(memoryStream.ToArray()),
                "file[]",
                faceImageUrl.FileName
            );

            HttpResponseMessage response = await _httpClient.PostAsync(
                FptEkycConfig.ApiFaceMatch,
                formDataContent
            );
            string resContentString = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new FptEkycException(FptEkycErrorCode.PersonalCustomerHttpError);
            }
            FaceMatchResponse faceMatchRes = JsonSerializer.Deserialize<FaceMatchResponse>(
                resContentString
            )!;
            FaceMatchData faceMatchData;
            if (faceMatchRes.Code == FaceMatchCodes.SUCCESS)
            {
                _logger.LogInformation(
                    $"Call Api face match success with status = {response.StatusCode}, response = {resContentString}"
                );
                faceMatchData = faceMatchRes.Data ?? new();
            }
            else if (faceMatchRes.Code == FaceMatchCodes.NO_FACES_DETECTED)
            {
                throw new FptEkycException(
                    FptEkycErrorCode.PersonalCustomerFaceRecognitionNoFaceDetected
                );
            }
            else
            {
                _logger.LogError("Call Api face match fail with response {0}", resContentString);
                throw new FptEkycException(FptEkycErrorCode.PersonalCustomerFaceMatch);
            }

            //if (!faceMatchData.IsMatch)
            //{
            //    throw new FaultException(new FaultReason("Xác thực khuôn mặt không khớp"), new FaultCode(((int)ErrorCode.InvestorFaceRecognitionNotMatch).ToString()), "");
            //}

            return faceMatchData;
        }

        /// <summary>
        /// Check nhỏ hơn 18 tuổi
        /// </summary>
        /// <param name="dob"></param>
        protected void CheckAge(DateTime? dob)
        {
            if (dob.HasValue)
            {
                double age = (DateTimeUtils.GetDate().Date - dob.Value.Date).Days / 365.0;
                var ageMin = _dbContext.SysVars.FirstOrDefault(o =>
                    o.GrName == GrNames.EKYC && o.VarName == VarNames.AGE_MIN
                );
                if (ageMin != null && age < double.Parse(ageMin.VarValue))
                {
                    throw new FptEkycException(FptEkycErrorCode.PersonalCustomerAgeInvalid);
                }
            }
        }

        /// <summary>
        /// Check ngày hết hạn có quá ngày hiện tại ko
        /// </summary>
        /// <param name="exp"></param>
        protected void CheckExp(DateTime? exp)
        {
            if (exp.HasValue && exp < DateTimeUtils.GetDate())
            {
                throw new FptEkycException(FptEkycErrorCode.PersonalCustomerIdExpired);
            }
        }

        /// <summary>
        /// Convert ngày cấp từ chuỗi sang datetime
        /// </summary>
        /// <param name="issueDate"></param>
        /// <returns></returns>
        protected DateTime? ConvertStringIssudeDateToDateTime(string issueDate)
        {
            var result = DateTimeUtils.FromDateStrDD_MM_YYYY_ToDate(issueDate);
            if (result == null)
            {
                result = DateTimeUtils.FromDateStrDD_MM_YY_ToDate(issueDate);
            }
            return result;
        }

        /// <summary>
        /// +15 năm từ ngày cấp nếu ngày hết hạn null (Mặc định là 15 năm)
        /// </summary>
        /// <param name="expDate"></param>
        /// <param name="issueDate"></param>
        /// <param name="docType"></param>
        /// <returns></returns>
        protected DateTime? ProccessExpDate(string expDate, DateTime? issueDate, string docType)
        {
            var result = DateTimeUtils.FromDateStrDD_MM_YYYY_ToDate(expDate);

            if (result == null && issueDate != null)
            {
                var cmnd = _dbContext.SysVars.FirstOrDefault(o =>
                    o.GrName == GrNames.EKYC && o.VarName == VarNames.CMND_EXPIRED_ADD_YEAR_IF_NULL
                );
                var year = cmnd != null ? int.Parse(cmnd.VarValue) : 0;
                if (docType == CustomerCardIdTypes.CCCD)
                {
                    var cccd = _dbContext.SysVars.FirstOrDefault(o =>
                        o.GrName == GrNames.EKYC
                        && o.VarName == VarNames.CCCD_EXPIRED_ADD_YEAR_IF_NULL
                    );
                    year = cccd != null ? int.Parse(cccd.VarValue) : 0;
                }
                result = issueDate.Value.AddYears(year);
            }
            return result;
        }

        public virtual Task<FaceMatchResultDto> Liveness(EkycLivenessDto input)
        {
            throw new NotImplementedException();
        }
    }
}
