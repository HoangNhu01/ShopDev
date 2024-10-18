using CR.Signature.Localization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CR.Signature.Configs
{
    public static class SignatureConfigStartUp
    {
        public static void ConfigureSignature(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<SignatureConfig>(
                builder.Configuration.GetSection("DigitalSignature")
            );
            builder.Services.AddSingleton<ISignatureLocalization, SignatureLocalization>();
            builder.Services.AddSingleton<ISignatureMapErrorCode, SignatureMapErrorCode>();
            builder.Services.AddScoped<ISignatureService, SignatureService>();
        }
    }
}
