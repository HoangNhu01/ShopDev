using CR.MeeyPartner.Localization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CR.MeeyPartner.Configs
{
    public static class MeeyPartnerConfigStartUp
    {
        public static void ConfigureMeeyPartner(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<MeeyPartnerConfig>(
                builder.Configuration.GetSection("MeeyPartnerConfig")
            );
            builder.Services.AddSingleton<IMeeyPartnerLocalization, MeeyPartnerLocalization>();
            builder.Services.AddSingleton<IMeeyPartnerMapErrorCode, MeeyPartnerMapErrorCode>();
            builder.Services.AddScoped<IMeeyPartnerService, MeeyPartnerService>();
        }
    }
}
