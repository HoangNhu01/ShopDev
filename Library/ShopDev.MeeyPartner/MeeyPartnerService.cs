using System.Net;
using System.Text;
using System.Text.Json;
using CR.MeeyPartner.Configs;
using CR.MeeyPartner.Constants;
using CR.MeeyPartner.Dtos;
using CR.MeeyPartner.Exceptions;
using CR.Utils.Net.MimeTypes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CR.MeeyPartner
{
    public class MeeyPartnerService : IMeeyPartnerService
    {
        private readonly ILogger<MeeyPartnerService> _logger;
        private readonly MeeyPartnerConfig _config;
        private readonly HttpClient _httpClient;

        public MeeyPartnerService(
            ILogger<MeeyPartnerService> logger,
            IOptions<MeeyPartnerConfig> config
        )
        {
            _logger = logger;
            _config = config.Value;
            _httpClient = InitHttpClient();
        }

        private HttpClient InitHttpClient()
        {
            HttpClient httpClient =
                new() { BaseAddress = new Uri(_config.BaseUrl), Timeout = TimeSpan.FromSeconds(5) };

            httpClient.DefaultRequestHeaders.Add("x-client-id", _config.ClientId);
            httpClient.DefaultRequestHeaders.Add("x-client-key", _config.ClientKey);
            return httpClient;
        }

        public async Task<string> SendOtp(string phone)
        {
            _logger.LogInformation($"{nameof(SendOtp)}: phone = {phone}");
            MeeyPartnerSendOtpRequestDto data = new() { Phone = phone, };

            StringContent content = new StringContent(
                JsonSerializer.Serialize(data),
                Encoding.UTF8,
                MimeTypeNames.ApplicationJson
            );
            HttpResponseMessage sendOtp = await _httpClient.PostAsync(
                MeeyPartnerConfig.ApiSendOtp,
                content
            );
            string resContentString = await sendOtp.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<MeeyPartnerSendOtpResponseDto>(
                resContentString
            )!;
            if (!sendOtp.IsSuccessStatusCode)
            {
                _logger.LogError(
                    $"{nameof(SendOtp)}: Call Api fail with status = {sendOtp.StatusCode}, response = {resContentString}"
                );
                switch (responseData.Error.Code?.ToString())
                {
                    case MeeyPartnerHandleErrorCode.SendOtpMutipleRequest:
                        throw new MeeyPartnerException(MeeyPartnerErrorCode.SendOtpMutipleRequest);
                    default:
                        throw new MeeyPartnerException(
                            MeeyPartnerErrorCode.SendOtpNotSuccess,
                            $"{responseData.Error.Message} ({responseData.Error.Code})"
                        );
                }
            }
            if (responseData.Data == null)
            {
                throw new MeeyPartnerException(MeeyPartnerErrorCode.SendOtpNotSuccess);
            }
            return responseData.Data.Data!;
        }

        public async Task<string> ResendOtp(string phone, string requestId)
        {
            _logger.LogInformation(
                $"{nameof(ResendOtp)}: phone = {phone}, requestId = {requestId}"
            );
            MeeyPartnerResendOtpRequestDto data = new() { Phone = phone, RequestId = requestId };

            StringContent content = new StringContent(
                JsonSerializer.Serialize(data),
                Encoding.UTF8,
                MimeTypeNames.ApplicationJson
            );
            HttpResponseMessage resendOtp = await _httpClient.PostAsync(
                MeeyPartnerConfig.ApiResendOtp,
                content
            );
            string resContentString = await resendOtp.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<MeeyPartnerResendOtpResponseDto>(
                resContentString
            )!;
            if (!resendOtp.IsSuccessStatusCode)
            {
                _logger.LogError(
                    $"{nameof(ResendOtp)}: Call Api fail with status = {resendOtp.StatusCode}, response = {resContentString}"
                );
                switch (responseData.Error.Code?.ToString())
                {
                    case MeeyPartnerHandleErrorCode.SendOtpMutipleRequest:
                        throw new MeeyPartnerException(MeeyPartnerErrorCode.SendOtpMutipleRequest);
                    default:
                        throw new MeeyPartnerException(
                            MeeyPartnerErrorCode.ResendOtpNotSuccess,
                            $"{responseData.Error.Message} ({responseData.Error.Code})"
                        );
                }
            }

            if (responseData.Data == null)
            {
                throw new MeeyPartnerException(MeeyPartnerErrorCode.ResendOtpNotSuccess);
            }
            return responseData.Data.RequestId!;
        }

        public async Task VerifyOtp(string phone, string requestId, string otp)
        {
            _logger.LogInformation(
                $"{nameof(VerifyOtp)}: phone = {phone}, requestId = {requestId}, otp = {otp}"
            );
            MeeyPartnerVerifyOtpRequestDto data =
                new()
                {
                    Phone = phone,
                    RequestId = requestId,
                    Otp = otp
                };

            StringContent content = new StringContent(
                JsonSerializer.Serialize(data),
                Encoding.UTF8,
                MimeTypeNames.ApplicationJson
            );
            HttpResponseMessage verifyOtp = await _httpClient.PostAsync(
                MeeyPartnerConfig.ApiVerifyOtp,
                content
            );
            string resContentString = await verifyOtp.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<MeeyPartnerVerifyOtpResponseDto>(
                resContentString
            )!;

            if (!verifyOtp.IsSuccessStatusCode)
            {
                _logger.LogError(
                    $"{nameof(VerifyOtp)}: Call Api fail with status = {verifyOtp.StatusCode}, response = {resContentString}"
                );
                MeeyPartnerHandleException(responseData.Error.Code, responseData.Error.Message);
            }
            if (responseData.Data == null)
            {
                throw new MeeyPartnerException(MeeyPartnerErrorCode.VerifyOtpNotSuccess);
            }
        }

        private void MeeyPartnerHandleException(object? code, string? message)
        {
            object errorMessage = code?.ToString() switch
            {
                MeeyPartnerHandleErrorCode.VerifyOtpInvalid
                    => throw new MeeyPartnerException(
                        MeeyPartnerErrorCode.VerifyOtpNotSuccess,
                        message
                    ),
                MeeyPartnerHandleErrorCode.VerifyOtpExpired
                    => throw new MeeyPartnerException(MeeyPartnerErrorCode.VerifyOtpExpired),
                MeeyPartnerHandleErrorCode.VerifyOtpTurnEnd
                    => throw new MeeyPartnerException(MeeyPartnerErrorCode.VerifyOtpTurnEnd),
                _
                    => throw new MeeyPartnerException(
                        MeeyPartnerErrorCode.VerifyOtpNotSuccess,
                        $"{message} ({code})"
                    )
            };
        }
    }
}
