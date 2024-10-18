using System.Net;
using System.Text;
using CR.Signature.Configs;
using CR.Signature.Constants;
using CR.Signature.Dtos;
using CR.Signature.Exceptions;
using CR.Utils.Net.MimeTypes;
using CR.Utils.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CR.Signature
{
    public class SignatureService : ISignatureService
    {
        private readonly ILogger _logger;
        private readonly SignatureConfig _config;

        public SignatureService(ILogger<SignatureService> logger, IOptions<SignatureConfig> config)
        {
            _logger = logger;
            _config = config.Value;
        }

        public async Task<Stream> FillDigitalSignature(
            Stream streamFile,
            string contractCodeGen,
            int? pageSign = 1
        )
        {
            streamFile.Position = 0;
            var ms = new MemoryStream();
            await streamFile.CopyToAsync(ms);
            string base64 = CryptographyUtils.StreamToBase64(ms);
            using HttpClient client = new() { Timeout = TimeSpan.FromSeconds(30) };
            client.BaseAddress = new Uri(_config.BaseUrl);
            var json = JsonConvert.SerializeObject(
                new DigitalSignatureRequest
                {
                    RequestID = Guid.NewGuid().ToString(),
                    SigningFileData = base64,
                    MimeType = MimeTypeNames.ApplicationPdf,
                    MetaDataSigning = new()
                    {
                        VisibleSignature = true,
                        ImageandText = false,
                        TextColor = "#175CD3",
                        Location = "Hà Nội",
                        SignReason = contractCodeGen,
                        Pageno = pageSign ?? 1,
                        SignatureBox = "50,50,250,120",
                        FontSize = 12,
                        SignatureImage = string.Empty
                    }
                }
            );
            var httpContent = new StringContent(json, Encoding.UTF8, MimeTypeNames.ApplicationJson);
            var response = await client.PostAsync(
                _config.BaseUrl + "api/signDocument",
                httpContent
            );
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    $"{nameof(FillDigitalSignature)}: responseBody = {await response.Content.ReadAsStringAsync()}, responseStatusCode = {response.StatusCode}"
                );
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new SignatureException(SignatureErrorCode.SignatureCanBeSign);
                }
                else if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                {
                    throw new SignatureException(SignatureErrorCode.TimeOutToSign);
                }
                else
                {
                    throw new SignatureException(SignatureErrorCode.InternalServerError);
                }
            }
            var signatureResponse = JsonConvert.DeserializeObject<DigitalSignatureResponse>(result);
            if (signatureResponse.ResponseCode == SignatureErrorCode.InnerMessageCode)
            {
                throw new SignatureException(SignatureErrorCode.InnerMessageCodeDetail);
            }
            MemoryStream stream = new MemoryStream(
                Convert.FromBase64String(signatureResponse.SignedFileData)
            );
            return stream;
        }
    }
}
