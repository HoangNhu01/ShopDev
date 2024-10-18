using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using CR.InfrastructureBase.Bank;
using CR.PVCB.Configs;
using CR.PVCB.Constants;
using CR.PVCB.Dtos.BankInfoDto;
using CR.PVCB.Dtos.DepositDto;
using CR.PVCB.Dtos.InfoAccountDto;
using CR.PVCB.Dtos.InquiryDtos;
using CR.PVCB.Dtos.TokenDtos;
using CR.PVCB.Dtos.VirtualAccDtos;
using CR.PVCB.Exceptions;
using CR.Utils.Net.MimeTypes;
using Google.Authenticator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CR.PVCB
{
    public class PvcbService : IPvcbService
    {
        private readonly ILogger _logger;
        private readonly PvcbConfig _config;
        private readonly PvcbSecrets _secrets;

        public PvcbService(
            ILogger<PvcbService> logger,
            IOptions<PvcbConfig> config,
            IOptions<PvcbSecrets> secrets
        )
        {
            _logger = logger;
            _config = config.Value;
            _secrets = secrets.Value;
        }

        private void PvcbHandleException(string code)
        {
            object errorMessage = code switch
            {
                PvcbHandleErrorCode.InnerCloseAccMessageCode
                    => throw new PvcbException(PvcbErrorCode.NotAllowClose),
                PvcbHandleErrorCode.InnerRegisterVAMessageCode
                    => throw new PvcbException(PvcbErrorCode.GetInforNotFound),
                PvcbHandleErrorCode.InnerTransactionMessageCode
                    => throw new PvcbException(PvcbErrorCode.GetTransactionVirtualAccNotFound),
                PvcbHandleErrorCode.InnerDetailVAMessageCode
                    => throw new PvcbException(PvcbErrorCode.GetVirtualAccNotFound),
                PvcbHandleErrorCode.InnerLockAccMessageCode
                    => throw new PvcbException(PvcbErrorCode.VAIsClosed),
                PvcbHandleErrorCode.InnerUnlockAccMessageCode
                    => throw new PvcbException(PvcbErrorCode.LiveRecordNotChange),
                PvcbHandleErrorCode.NoDataFound
                    => throw new PvcbException(PvcbErrorCode.NoDataFound),
                PvcbHandleErrorCode.TransactionIdNotFound
                    => throw new PvcbException(PvcbErrorCode.TransactionIdNotFound),
                PvcbHandleErrorCode.OriginalPaymentNeverReceived
                    => throw new PvcbException(PvcbErrorCode.OriginalPaymentNeverReceived),
                PvcbHandleErrorCode.JSONRequestInvalid
                    => throw new PvcbException(PvcbErrorCode.JSONRequestInvalid),
                PvcbHandleErrorCode.AmountExceededDailyTransactionLimit
                    => throw new PvcbException(PvcbErrorCode.AmountExceededDailyTransactionLimit),
                PvcbHandleErrorCode.TransactionAmountExceedsTheAllowedLimit
                    => throw new PvcbException(
                        PvcbErrorCode.TransactionAmountExceedsTheAllowedLimit
                    ),
                PvcbHandleErrorCode.TransactionAmountBelowAllowable
                    => throw new PvcbException(PvcbErrorCode.TransactionAmountBelowAllowable),
                PvcbHandleErrorCode.TheAmountstringheAccountIsNotEnoughToMakeTheTransaction
                    => throw new PvcbException(
                        PvcbErrorCode.TheAmountInTheAccountIsNotEnoughToMakeTheTransaction
                    ),
                PvcbHandleErrorCode.InvalidTransactionAmount
                    => throw new PvcbException(PvcbErrorCode.InvalidTransactionAmount),
                PvcbHandleErrorCode.CardNumberOrAccountNotFound
                    => throw new PvcbException(PvcbErrorCode.CardNumberOrAccountNotFound),
                PvcbHandleErrorCode.CardNumberOrAccountNotCorrect
                    => throw new PvcbException(PvcbErrorCode.CardNumberOrAccountNotCorrect),
                PvcbHandleErrorCode.CardOrAccountStatusNotValid
                    => throw new PvcbException(PvcbErrorCode.CardOrAccountStatusNotValid),
                PvcbHandleErrorCode.CardOrccountTypeNotValid
                    => throw new PvcbException(PvcbErrorCode.CardOrccountTypeNotValid),
                PvcbHandleErrorCode.NameOnCardOrAccountNotCorrect
                    => throw new PvcbException(PvcbErrorCode.NameOnCardOrAccountNotCorrect),
                PvcbHandleErrorCode.CardOrAccountNotActivated
                    => throw new PvcbException(PvcbErrorCode.CardOrAccountNotActivated),
                PvcbHandleErrorCode.CardOrAccountHasBeenBlocked
                    => throw new PvcbException(PvcbErrorCode.CardOrAccountHasBeenBlocked),
                PvcbHandleErrorCode.InsufficientFunds
                    => throw new PvcbException(PvcbErrorCode.InsufficientFunds),
                PvcbHandleErrorCode.BeneficiaryBankIsNotValidOrHasNotJoinedTheService
                    => throw new PvcbException(
                        PvcbErrorCode.BeneficiaryBankIsNotValidOrHasNotJoinedTheService
                    ),
                PvcbHandleErrorCode.ExpiredBeneficiaryCard
                    => throw new PvcbException(PvcbErrorCode.ExpiredBeneficiaryCard),
                PvcbHandleErrorCode.TransactionFailedBecauseDestinationCardIsInLostStatus
                    => throw new PvcbException(
                        PvcbErrorCode.TransactionFailedBecauseDestinationCardIsInLostStatus
                    ),
                PvcbHandleErrorCode.CardNumberOrAccountHasExpired
                    => throw new PvcbException(PvcbErrorCode.CardNumberOrAccountHasExpired),
                PvcbHandleErrorCode.CardNumberIsInLostStatus
                    => throw new PvcbException(PvcbErrorCode.CardNumberIsInLostStatus),
                PvcbHandleErrorCode.TransactionAmountExceedsTheAllowedLimitForTheDay
                    => throw new PvcbException(
                        PvcbErrorCode.TransactionAmountExceedsTheAllowedLimitForTheDay
                    ),
                PvcbHandleErrorCode.NumberOfTransactionsRxceedingTheAllowedLimitInADay
                    => throw new PvcbException(
                        PvcbErrorCode.NumberOfTransactionsRxceedingTheAllowedLimitInADay
                    ),
                PvcbHandleErrorCode.TimeoutFromBeneficiaryBank
                    => throw new PvcbException(PvcbErrorCode.TimeoutFromBeneficiaryBank),
                PvcbHandleErrorCode.NoTransactionProcessingStatusMessagesFromNapas
                    => throw new PvcbException(
                        PvcbErrorCode.NoTransactionProcessingStatusMessagesFromNapas
                    ),
                PvcbHandleErrorCode.ThisTransactionDoesNotSupportRefund
                    => throw new PvcbException(PvcbErrorCode.ThisTransactionDoesNotSupportRefund),
                PvcbHandleErrorCode.NotEligibleToUseTheService
                    => throw new PvcbException(PvcbErrorCode.NotEligibleToUseTheService),
                PvcbHandleErrorCode.TransactionIsDuplicate
                    => throw new PvcbException(PvcbErrorCode.TransactionIsDuplicate),
                PvcbHandleErrorCode.ClientIDInvalid
                    => throw new PvcbException(PvcbErrorCode.ClientIDInvalid),
                PvcbHandleErrorCode.DataCreditorAccountSourceNumberMustNotBeNull
                    => throw new PvcbException(
                        PvcbErrorCode.DataCreditorAccountSourceNumberMustNotBeNull
                    ),
                PvcbHandleErrorCode.DataCreditorAccountSourceTypeMustNotBeNull
                    => throw new PvcbException(
                        PvcbErrorCode.DataCreditorAccountSourceTypeMustNotBeNull
                    ),
                PvcbHandleErrorCode.RiskBINIdMustNotBeNull
                    => throw new PvcbException(PvcbErrorCode.RiskBINIdMustNotBeNull),
                PvcbHandleErrorCode.DataCreditorAccountSourceTypeBeneficiaryAccountTypeACCAccountOrPANCard
                    => throw new PvcbException(
                        PvcbErrorCode.DataCreditorAccountSourceTypeBeneficiaryAccountTypeACCAccountOrPANCard
                    ),
                PvcbHandleErrorCode.DataTransIdMustNotBeNull
                    => throw new PvcbException(PvcbErrorCode.DataTransIdMustNotBeNull),
                PvcbHandleErrorCode.DataDateTimeMustNotBeNull
                    => throw new PvcbException(PvcbErrorCode.DataDateTimeMustNotBeNull),
                PvcbHandleErrorCode.DataCreditorAccountSourceNumberMustNotBeNull2
                    => throw new PvcbException(
                        PvcbErrorCode.DataCreditorAccountSourceNumberMustNotBeNull2
                    ),
                PvcbHandleErrorCode.DataCreditorAccountSourceTypeMustNotBeNull2
                    => throw new PvcbException(
                        PvcbErrorCode.DataCreditorAccountSourceTypeMustNotBeNull2
                    ),
                PvcbHandleErrorCode.DataInstructedAmountAmountMustNotBeNull
                    => throw new PvcbException(
                        PvcbErrorCode.DataInstructedAmountAmountMustNotBeNull
                    ),
                PvcbHandleErrorCode.DataInstructedAmountAmountIsNotNumber
                    => throw new PvcbException(PvcbErrorCode.DataInstructedAmountAmountIsNotNumber),
                PvcbHandleErrorCode.RiskBINIdMustNotBeNull2
                    => throw new PvcbException(PvcbErrorCode.RiskBINIdMustNotBeNull2),
                PvcbHandleErrorCode.DataTransIdAlreadyExists
                    => throw new PvcbException(PvcbErrorCode.DataTransIdAlreadyExists),
                PvcbHandleErrorCode.DataDateTimeIsNotInTheCorrectFormat
                    => throw new PvcbException(PvcbErrorCode.DataDateTimeIsNotInTheCorrectFormat),
                PvcbHandleErrorCode.DataCreditorAccountSourceTypeBeneficiaryAccountTypeACCAccountPANCard
                    => throw new PvcbException(
                        PvcbErrorCode.DataCreditorAccountSourceTypeBeneficiaryAccountTypeACCAccountPANCard
                    ),
                PvcbHandleErrorCode.DataInstructedAmountAmountLimitPerTime
                    => throw new PvcbException(
                        PvcbErrorCode.DataInstructedAmountAmountLimitPerTime
                    ),
                PvcbHandleErrorCode.RiskTransDescNeedsToBeLessThan210Characters
                    => throw new PvcbException(
                        PvcbErrorCode.RiskTransDescNeedsToBeLessThan210Characters
                    ),
                PvcbHandleErrorCode.RiskMustNotBeNull
                    => throw new PvcbException(PvcbErrorCode.RiskMustNotBeNull),
                PvcbHandleErrorCode.RiskTransDescMustNotBeNull
                    => throw new PvcbException(PvcbErrorCode.RiskTransDescMustNotBeNull),
                PvcbHandleErrorCode.DataCreditorAccountSourceNameMustNotBeNull
                    => throw new PvcbException(
                        PvcbErrorCode.DataCreditorAccountSourceNameMustNotBeNull
                    ),
                PvcbHandleErrorCode.RiskTransDescDoNotUseSpecialCharacters
                    => throw new PvcbException(
                        PvcbErrorCode.RiskTransDescDoNotUseSpecialCharacters
                    ),
                PvcbHandleErrorCode.DataTransIdCannotBeANumber
                    => throw new PvcbException(PvcbErrorCode.DataTransIdCannotBeANumber),
                PvcbHandleErrorCode.DataTransIdWrongFormat
                    => throw new PvcbException(PvcbErrorCode.DataTransIdWrongFormat),
                PvcbHandleErrorCode.DataCreditorAccountSourceNumberCannotBeANumber
                    => throw new PvcbException(
                        PvcbErrorCode.DataCreditorAccountSourceNumberCannotBeANumber
                    ),
                PvcbHandleErrorCode.DataCreditorAccountSourceNumberWrongFormat
                    => throw new PvcbException(
                        PvcbErrorCode.DataCreditorAccountSourceNumberWrongFormat
                    ),
                PvcbHandleErrorCode.RiskTransDescCannotBeANumber
                    => throw new PvcbException(PvcbErrorCode.RiskTransDescCannotBeANumber),
                PvcbHandleErrorCode.DataCreditorAccountSourceNameCannotBeANumber
                    => throw new PvcbException(
                        PvcbErrorCode.DataCreditorAccountSourceNameCannotBeANumber
                    ),
                PvcbHandleErrorCode.SystemErrorPleaseTrAagainLater
                    => throw new PvcbException(PvcbErrorCode.SystemErrorPleaseTrAagainLater),
                PvcbHandleErrorCode.SystemError
                    => throw new PvcbException(PvcbErrorCode.SystemError),
                PvcbHandleErrorCode.UnableToConnectToNapasSystem
                    => throw new PvcbException(PvcbErrorCode.UnableToConnectToNapasSystem),
                PvcbHandleErrorCode.UnableToParseMessageFromNapas
                    => throw new PvcbException(PvcbErrorCode.UnableToParseMessageFromNapas),
                PvcbHandleErrorCode.UnableToConnectToT24System
                    => throw new PvcbException(PvcbErrorCode.UnableToConnectToT24System),
                PvcbHandleErrorCode.UnableToConnectToERSSystem
                    => throw new PvcbException(PvcbErrorCode.UnableToConnectToERSSystem),
                PvcbHandleErrorCode.UnableToConnectToFESystem
                    => throw new PvcbException(PvcbErrorCode.UnableToConnectToFESystem),
                PvcbHandleErrorCode.Timeout => throw new PvcbException(PvcbErrorCode.Timeout),
                PvcbHandleErrorCode.TransactionsRejectedByInternalSystems
                    => throw new PvcbException(PvcbErrorCode.TransactionsRejectedByInternalSystems),
                PvcbHandleErrorCode.TransactionsRejectedByExternalSystems
                    => throw new PvcbException(PvcbErrorCode.TransactionsRejectedByExternalSystems),
                PvcbHandleErrorCode.TransactionPendingPayment
                    => throw new PvcbException(PvcbErrorCode.TransactionPendingPayment),
                PvcbHandleErrorCode.TheTransactionHasBeenRefunded
                    => throw new PvcbException(PvcbErrorCode.TheTransactionHasBeenRefunded),
                PvcbHandleErrorCode.BackendResponseSuccess
                    => throw new PvcbException(PvcbErrorCode.BackendResponseSuccess),
                PvcbHandleErrorCode.MACInvalid => throw new PvcbException(PvcbErrorCode.MACInvalid),
                PvcbHandleErrorCode.GenerateMACFailure
                    => throw new PvcbException(PvcbErrorCode.GenerateMACFailure),
                PvcbHandleErrorCode.WaitForConfirmation
                    => throw new PvcbException(PvcbErrorCode.WaitForConfirmation),
                PvcbHandleErrorCode.UnableToGetMessageFromNapasSystem
                    => throw new PvcbException(PvcbErrorCode.UnableToGetMessageFromNapasSystem),
                PvcbHandleErrorCode.SystemHasAnErrorPleaseContactBankForMoreDetails
                    => throw new PvcbException(
                        PvcbErrorCode.SystemHasAnErrorPleaseContactBankForMoreDetails
                    ),
                _ => throw new PvcbException(PvcbErrorCode.PvcbBadRequest)
            };
        }

        public async Task<PvcbBankInfoResponseDto> GetListBankFromPvcb()
        {
            _logger.LogInformation($"{nameof(GetListBankFromPvcb)}:");
            using HttpClient client = new();
            client.BaseAddress = new Uri(_config.BaseUrl);
            var token = await GetPvcbToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                token.access_token
            );
            HttpResponseMessage response = await client.GetAsync(PvcbConfig.GetListBankPath);
            var result = await response.Content.ReadFromJsonAsync<PvcbBankInfoResponseDto>();
            return result ?? new();
        }

        public async Task<PvcbGetTokenReponseDto> GetPvcbToken()
        {
            _logger.LogInformation($"{nameof(GetPvcbToken)}:");
            using HttpClient client = new();
            client.BaseAddress = new Uri(_config.TokenUrl);
            var formData = new Dictionary<string, string>
            {
                { "grant_type", PvcbConfig.GrantType },
                { "client_id", _secrets.ClientId },
                { "client_secret", _secrets.ClientSecret }
            };
            HttpResponseMessage response = await client.PostAsync(
                PvcbConfig.GetTokenPath,
                new FormUrlEncodedContent(formData)
            );
            // Đọc và hiển thị kết quả
            var responseBody = await response.Content.ReadFromJsonAsync<PvcbGetTokenReponseDto>();
            return responseBody ?? new();
        }

        public async Task OpenVirtualAcc(string vaName, string vaNumber)
        {
            _logger.LogInformation(
                $"{nameof(OpenVirtualAcc)}:vaName = {vaName}; vaNumber = {vaNumber}"
            );
            using HttpClient client = new() { Timeout = TimeSpan.FromSeconds(5) };
            client.BaseAddress = new Uri(_config.BaseUrl);
            var token = await GetPvcbToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                token.access_token
            );
            client.DefaultRequestHeaders.Add(PvcbConfig.XIdempotencyKey, Guid.NewGuid().ToString());
            PvcVirtualAccRequestDto accRequestDto =
                new()
                {
                    Channel = string.Empty,
                    NumberRule = RegulationParameters.NumberRuleManual,
                    NameRule = RegulationParameters.NameRuleManual,
                    NumofVA = PvcbVAProp.DefaultNumofVA,
                    CreateVABatch = new()
                    {
                        new()
                        {
                            VaDetail = new() { VaName = vaName, VaNumber = vaNumber }
                        }
                    },
                    Description = "Open batch virtual acc"
                };
            HttpResponseMessage response = await client.PostAsync(
                PvcbConfig.OpenVirtualAcc,
                new StringContent(
                    JsonSerializer.Serialize(accRequestDto),
                    Encoding.UTF8,
                    MimeTypeNames.ApplicationJson
                )
            );
            var result = await response.Content.ReadAsStringAsync();
            var accReponseDatas =
                JsonSerializer.Deserialize<
                    PvcVirtualResponeBaseDto<PvcRegisterVirtualAccReponseData>
                >(result) ?? throw new PvcbException(PvcbErrorCode.PvcbError);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    $"{nameof(OpenVirtualAcc)}: responseBody = {result}, responseStatusCode = {response.StatusCode}"
                );
                throw new PvcbException(PvcbErrorCode.AnErrorOccurred);
            }
            else if (
                response.IsSuccessStatusCode
                && accReponseDatas.Code != PvcbHandleErrorCode.SuccessCode
            )
            {
                _logger.LogError(
                    $"{nameof(OpenVirtualAcc)}: responseBody = {result}, responseStatusCode = {response.StatusCode}"
                );
                PvcbHandleException(accReponseDatas.Code);
            }
        }

        public async Task<PvcVirtualResponeBaseDto<PvcVirtualAccDto>> GetDetailVirtualAcc(
            string accNumber
        )
        {
            _logger.LogInformation($"{nameof(GetDetailVirtualAcc)}: acctNumber:{accNumber}");
            using HttpClient client = new() { Timeout = TimeSpan.FromSeconds(5) };
            client.BaseAddress = new Uri(_config.BaseUrl);
            var token = await GetPvcbToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                token.access_token
            );
            HttpResponseMessage response = await client.GetAsync(
                PvcbConfig.GetDetailVirtualAcc + accNumber
            );
            var result = await response.Content.ReadAsStringAsync();
            var accReponseDatas =
                JsonSerializer.Deserialize<PvcVirtualResponeBaseDto<PvcVirtualAccDto>>(result)
                ?? throw new PvcbException(PvcbErrorCode.PvcbError);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    $"{nameof(GetDetailVirtualAcc)}: responseBody = {result}, responseStatusCode = {response.StatusCode}"
                );
                PvcbHandleException(accReponseDatas.Code);
            }
            return accReponseDatas;
        }

        public async Task<
            PvcVirtualResponeBaseDto<PvcVirtualAccTransactionDto>
        > GetListTransactionOfVirtualAcc(string accNumber, string dateTime)
        {
            _logger.LogInformation(
                $"{nameof(GetListTransactionOfVirtualAcc)}: accNumber:{accNumber}; dateTime:{dateTime}"
            );
            using HttpClient client = new() { Timeout = TimeSpan.FromSeconds(5) };
            client.BaseAddress = new Uri(_config.BaseUrl);
            var token = await GetPvcbToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                token.access_token
            );
            HttpResponseMessage response = await client.GetAsync(
                PvcbConfig.GetListTransactionOfVirtualAcc + accNumber + $"?dateTime={dateTime}"
            );
            var result = await response.Content.ReadAsStringAsync();
            var transAccReponseDatas =
                JsonSerializer.Deserialize<PvcVirtualResponeBaseDto<PvcVirtualAccTransactionDto>>(
                    result
                ) ?? throw new PvcbException(PvcbErrorCode.PvcbError);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    $"{nameof(GetListTransactionOfVirtualAcc)}: responseBody = {result}, responseStatusCode = {response.StatusCode}"
                );
                PvcbHandleException(transAccReponseDatas.Code);
            }
            return transAccReponseDatas;
        }

        public async Task<PvcVirtualResponeBaseDto> LockVirtualAcc(
            string accNumber,
            string lockReason
        )
        {
            _logger.LogInformation(
                $"{nameof(LockVirtualAcc)}: acctNumber:{accNumber}; lockReason:{lockReason}"
            );
            using HttpClient client = new() { Timeout = TimeSpan.FromSeconds(5) };
            client.BaseAddress = new Uri(_config.BaseUrl);
            var token = await GetPvcbToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                token.access_token
            );
            client.DefaultRequestHeaders.Add(PvcbConfig.XIdempotencyKey, Guid.NewGuid().ToString());
            HttpResponseMessage response = await client.PostAsync(
                PvcbConfig.LockVirtualAcc + accNumber,
                new StringContent(
                    JsonSerializer.Serialize(lockReason),
                    Encoding.UTF8,
                    MimeTypeNames.ApplicationJson
                )
            );
            var result = await response.Content.ReadAsStringAsync();
            var accReponseDatas =
                JsonSerializer.Deserialize<PvcVirtualResponeBaseDto>(result)
                ?? throw new PvcbException(PvcbErrorCode.PvcbError);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    $"{nameof(LockVirtualAcc)}: responseBody = {result}, responseStatusCode = {response.StatusCode}"
                );
                PvcbHandleException(accReponseDatas.Code);
            }
            return accReponseDatas;
        }

        public async Task<PvcVirtualResponeBaseDto> UnLockVirtualAcc(string accNumber)
        {
            _logger.LogInformation($"{nameof(UnLockVirtualAcc)}: acctNumber:{accNumber}");
            using HttpClient client = new() { Timeout = TimeSpan.FromSeconds(5) };
            client.BaseAddress = new Uri(_config.BaseUrl);
            var token = await GetPvcbToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                token.access_token
            );
            HttpResponseMessage response = await client.PostAsync(
                PvcbConfig.UnLockVirtualAcc + accNumber,
                new StringContent(string.Empty)
            );
            var result = await response.Content.ReadAsStringAsync();
            var accReponseDatas =
                JsonSerializer.Deserialize<PvcVirtualResponeBaseDto>(result)
                ?? throw new PvcbException(PvcbErrorCode.PvcbError);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    $"{nameof(UnLockVirtualAcc)}: responseBody = {result}, responseStatusCode = {response.StatusCode}"
                );
                PvcbHandleException(accReponseDatas.Code);
            }
            return accReponseDatas;
        }

        public async Task<PvcVirtualResponeBaseDto> CloseVirtualAcc(string accNumber)
        {
            _logger.LogInformation($"{nameof(CloseVirtualAcc)}: acctNumber:{accNumber}");
            using HttpClient client = new() { Timeout = TimeSpan.FromSeconds(5) };
            client.BaseAddress = new Uri(_config.BaseUrl);
            var token = await GetPvcbToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                token.access_token
            );
            client.DefaultRequestHeaders.Add(PvcbConfig.XIdempotencyKey, Guid.NewGuid().ToString());
            HttpResponseMessage response = await client.PostAsync(
                PvcbConfig.CloseVirtualAcc + accNumber,
                new StringContent(string.Empty)
            );
            var result = await response.Content.ReadAsStringAsync();
            var accReponseDatas =
                JsonSerializer.Deserialize<PvcVirtualResponeBaseDto>(result)
                ?? throw new PvcbException(PvcbErrorCode.PvcbError);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    $"{nameof(CloseVirtualAcc)}: responseBody = {result}, responseStatusCode = {response.StatusCode}"
                );
                PvcbHandleException(accReponseDatas.Code);
            }
            return accReponseDatas;
        }

        public async Task<PvcbInquiryResponseDto> Inquiry(PvcbInquiryRequestDto input)
        {
            _logger.LogInformation($"{nameof(Inquiry)}: input = {JsonSerializer.Serialize(input)}");
            using HttpClient client = new() { Timeout = TimeSpan.FromSeconds(5) };
            client.BaseAddress = new Uri(_config.BaseUrl);
            var token = await GetPvcbToken();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                token.access_token
            );
            var stringContent = new StringContent(
                JsonSerializer.Serialize(input),
                Encoding.UTF8,
                MimeTypeNames.ApplicationJson
            );

            HttpResponseMessage response = await client.PostAsync(
                PvcbConfig.InquiryPath,
                stringContent
            );
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var errorMessage = await response.Content.ReadFromJsonAsync<PvcbInquiryErrorDto>();
                _logger.LogError(
                    $"{nameof(Inquiry)}: responseBody = {await response.Content.ReadAsStringAsync()}, responseStatusCode = {response.StatusCode}"
                );
                if (
                    errorMessage != null
                    && errorMessage.Message != null
                    && errorMessage.Code != null
                )
                {
                    PvcbHandleException(errorMessage.Code);
                }
                else
                {
                    throw new PvcbException(PvcbErrorCode.PvcbError);
                }
            }
            else
            {
                var responseBody =
                    await response.Content.ReadFromJsonAsync<PvcbInquiryResponseDto>();
                if (responseBody == null)
                {
                    throw new PvcbException(PvcbErrorCode.PvcbError);
                }
                else if (responseBody.Data == null)
                {
                    throw new PvcbException(PvcbErrorCode.PvcbResponseDataNull);
                }
                else if (responseBody.Data.CreditorAccount == null)
                {
                    throw new PvcbException(PvcbErrorCode.PvcbResponseCredittorAccountNull);
                }
                else if (responseBody.Data.CreditorAccount.SourceName == null)
                {
                    throw new PvcbException(PvcbErrorCode.PvcbResponseSourceNameNull);
                }
                return responseBody;
            }
            return new();
        }

        public async Task<RepsonseInfoAcountDto> PvcbInfoAccount()
        {
            _logger.LogInformation($"{nameof(PvcbInfoAccount)}:");
            using HttpClient client = new();
            client.BaseAddress = new Uri(_config.BaseUrl);
            var token = await GetPvcbToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                token.access_token
            );
            HttpResponseMessage response = await client.GetAsync(PvcbConfig.GetInfoAccount);
            // Đọc và hiển thị kết quả
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var errorMessage = await response.Content.ReadFromJsonAsync<PvcbInquiryErrorDto>();
                _logger.LogError(
                    $"{nameof(PvcbInfoAccount)}: responseBody = {await response.Content.ReadAsStringAsync()}, responseStatusCode = {response.StatusCode}"
                );
                if (
                    errorMessage != null
                    && errorMessage.Message != null
                    && errorMessage.Code != null
                )
                {
                    PvcbHandleException(errorMessage.Code);
                }
                else
                {
                    throw new PvcbException(PvcbErrorCode.PvcbError);
                }
            }
            else
            {
                var responseBody =
                    await response.Content.ReadFromJsonAsync<RepsonseInfoAcountDto>();
                return responseBody ?? new();
            }
            return new();
        }

        public async Task<PvcbDepositResponseDto> Deposit(PvcbDepositRequestDto input)
        {
            _logger.LogInformation($"{nameof(Deposit)}: input = {JsonSerializer.Serialize(input)}");
            using HttpClient client = new() { Timeout = TimeSpan.FromSeconds(5) };
            client.BaseAddress = new Uri(_config.BaseUrl);
            var token = await GetPvcbToken();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                token.access_token
            );
            client.DefaultRequestHeaders.Add(PvcbConfig.XIdempotencyKey, Guid.NewGuid().ToString());

            var stringContent = new StringContent(
                JsonSerializer.Serialize(input),
                Encoding.UTF8,
                MimeTypeNames.ApplicationJson
            );
            HttpResponseMessage response = await client.PostAsync(
                PvcbConfig.DepositPath,
                stringContent
            );
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var errorMessage = await response.Content.ReadFromJsonAsync<PvcbDepositErrorDto>();
                _logger.LogError(
                    $"{nameof(Deposit)}: responseBody = {await response.Content.ReadAsStringAsync()}, responseStatusCode = {response.StatusCode}"
                );
                if (errorMessage != null && errorMessage.Code != null)
                {
                    PvcbHandleException(errorMessage.Code);
                }
                else
                {
                    throw new PvcbException(PvcbErrorCode.PvcbError);
                }
            }
            else
            {
                var responseBody =
                    await response.Content.ReadFromJsonAsync<PvcbDepositResponseDto>();
                if (responseBody == null)
                {
                    throw new PvcbException(PvcbErrorCode.PvcbError);
                }
                else if (responseBody.Data == null)
                {
                    throw new PvcbException(PvcbErrorCode.PvcbResponseDataNull);
                }
                else if (responseBody.Data.CreditorAccount == null)
                {
                    throw new PvcbException(PvcbErrorCode.PvcbResponseCredittorAccountNull);
                }
                else if (responseBody.Data.CreditorAccount.SourceName == null)
                {
                    throw new PvcbException(PvcbErrorCode.PvcbResponseSourceNameNull);
                }
                return responseBody;
            }
            return new();
        }

        public bool VerifyGoogleAuthOtp(string otp)
        {
            TwoFactorAuthenticator tfa = new();
            return tfa.ValidateTwoFactorPIN(_secrets.GoogleAuthenticator.Secret, otp);
        }

        public string GetGoogleAuthQrBase64()
        {
            TwoFactorAuthenticator tfa = new();
            SetupCode setupInfo = tfa.GenerateSetupCode(
                _secrets.GoogleAuthenticator.Issuer,
                _secrets.GoogleAuthenticator.AccountTitle,
                _secrets.GoogleAuthenticator.Secret,
                false,
                3
            );
            return setupInfo.QrCodeSetupImageUrl;
        }
    }
}
