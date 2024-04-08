using CR.FptEkyc.Localization;
using CR.InfrastructureBase.Ekyc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CR.FptEkyc.Configs
{
    public static class FptEkycConfigStartup
    {
        public static void ConfigureFptEkyc(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<FptEkycConfig>(
                builder.Configuration.GetSection("FptEkycConfig")
            );
            builder.Services.AddSingleton<IFptEkycLocalization, FptEkycLocalization>();
            builder.Services.AddSingleton<IFptEkycMapErrorCode, FptEkycMapErrorCode>();
            builder.Services.AddScoped<IEkyc, FptEkycService>();
        }
    }
}
