using System.Net;
using System.Text.Json;
using CR.PVCB.Configs;
using CR.PVCB.Constants;
using CR.PVCB.Dtos.EkycDtos;
using CR.PVCB.Exceptions;
using CR.Utils.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CR.PVCB
{
    public class PvcbEkycService : IPvcbEkycService
    {
        private readonly PvcbEkycConfig _config;
        private readonly PvcbEkycSecrets _secrets;
        private readonly ILogger<PvcbEkycService> _logger;

        public PvcbEkycService(
            ILogger<PvcbEkycService> logger,
            IOptions<PvcbEkycConfig> config,
            IOptions<PvcbEkycSecrets> secrets
        )
        {
            _config = config.Value;
            _secrets = secrets.Value;
            _logger = logger;
        }

        private async Task<PvcbEkycResponseTokenDto> GetToken()
        {
            _logger.LogInformation($"{nameof(GetToken)}");

            Dictionary<string, string> requestBody =
                new()
                {
                    { "grant_type", "client_credentials" },
                    { "client_id", _secrets.ClientId },
                    { "client_secret", _secrets.ClientSecret },
                };

            using HttpClient httpClient = new();
            httpClient.BaseAddress = new Uri(_config.TokenUrl);
            HttpResponseMessage resOCRApi = await httpClient.PostAsync(
                PvcbEkycConfig.GetTokenPath,
                new FormUrlEncodedContent(requestBody)
            );
            string resContentString = await resOCRApi.Content.ReadAsStringAsync();
            if (resOCRApi.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(
                    $"{nameof(GetToken)}: Call Api fail with status = {resOCRApi.StatusCode}, response = {resContentString}"
                );
                throw new PvcbException(PvcbErrorCode.EkycErrorHttpRequest);
            }
            var responseData = JsonSerializer.Deserialize<PvcbEkycResponseTokenDto>(
                resContentString
            )!;
            return responseData;
        }

        public async Task<PvcbEkycResponseInformationDto> GetCustomerInformation(
            string userIdPartner
        )
        {
            _logger.LogInformation(
                $"{nameof(GetCustomerInformation)}, userIdPartner = {userIdPartner}"
            );
            var token = await GetToken();

            using HttpClient httpClient = new();
            httpClient.BaseAddress = new Uri(_config.BaseUrl);
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.AccessToken);
            HttpResponseMessage resOCRApi = await httpClient.GetAsync(
                PvcbEkycConfig.GetInformationPath + $"/{userIdPartner}"
            );
            string resContentString = await resOCRApi.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<PvcbEkycResponseDto>(resContentString)!;
            if (resOCRApi.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError(
                    $"{nameof(GetCustomerInformation)}: Call Api fail with status = {resOCRApi.StatusCode}, response = {resContentString}"
                );
            }
            switch (resOCRApi.StatusCode)
            {
                case HttpStatusCode.OK:
                    break;
                case HttpStatusCode.NotFound:
                    throw new PvcbException(
                        PvcbErrorCode.EkycErrorGetInformationNotFound,
                        responseData?.Message + $" ({responseData?.Code})"
                    );
                case HttpStatusCode.Unauthorized:
                    throw new PvcbException(
                        PvcbErrorCode.EkycErrorGetInformationUnauthorized,
                        responseData?.Message + $" ({responseData?.Code})"
                    );
                default:
                    throw new PvcbException(PvcbErrorCode.EkycErrorHttpRequest);
            }
            if (responseData.Data == null)
            {
                throw new PvcbException(PvcbErrorCode.EkycErrorGetInformationNotFound);
            }
            var decryptData = CryptographyUtils.DecryptAES256CBC(
                responseData.Data,
                _secrets.EkycDecrypt.DecryptKey,
                _secrets.EkycDecrypt.DecryptIv
            );
            var result = JsonSerializer.Deserialize<PvcbEkycResponseInformationDto>(decryptData)!;
            if (result == null)
            {
                throw new PvcbException(PvcbErrorCode.EkycErrorGetInformationNotFound);
            }
            return result;
        }
    }
}
