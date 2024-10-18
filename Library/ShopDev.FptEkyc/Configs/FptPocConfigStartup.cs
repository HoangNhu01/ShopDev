using CR.FptEkyc.Localization;
using CR.InfrastructureBase.Ekyc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CR.FptEkyc.Configs
{
    public static class FptPocConfigStartup
    {
        public static void ConfigureFptPoc(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<FptPocConfig>(
                builder.Configuration.GetSection("FptPocConfig")
            );
            builder.Services.AddSingleton<IFptEkycLocalization, FptEkycLocalization>();
            builder.Services.AddSingleton<IFptEkycMapErrorCode, FptEkycMapErrorCode>();
            builder.Services.AddScoped<IFptPocService, FptPocService>();
            builder.Services.AddScoped<IEkyc, FptPocService>();
        }
    }
}
