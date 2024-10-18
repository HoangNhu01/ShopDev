using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ShopDev.ConvertFile.Localization;

namespace ShopDev.ConvertFile.Configs
{
    public static class ConvertFileConfigStartup
    {
        public static void ConfigureConvertFile(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<ConvertFileConfig>(
                builder.Configuration.GetSection("ConvertFile")
            );
            builder.Services.AddSingleton<IConvertFileLocalization, ConvertFileLocalization>();
            builder.Services.AddSingleton<IConvertFileMapErrorCode, ConvertFileMapErrorCode>();
            builder.Services.AddScoped<IConvertFileService, ConvertFileService>();
        }
    }
}
