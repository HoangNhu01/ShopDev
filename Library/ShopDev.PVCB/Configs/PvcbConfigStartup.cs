using System.Text;
using CR.Constants.Environments;
using CR.PVCB.Localization;
using Hangfire.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CR.PVCB.Configs
{
    public static class PvcbConfigStartup
    {
        public static void ConfigurePvcb(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<PvcbConfig>(builder.Configuration.GetSection("PvcbConfig"));
            string pvcbSecretsJson;
            if (
                new string[] { EnvironmentNames.Development, EnvironmentNames.Staging }.Contains(
                    builder.Environment.EnvironmentName
                )
            )
            {
                pvcbSecretsJson = "secrets.Development.json";
            }
            else
            {
                pvcbSecretsJson = "secrets.json";
            }
            builder.Services.Configure<PvcbSecrets>(
                new ConfigurationBuilder()
                    .AddJsonStream(
                        new MemoryStream(
                            Encoding.UTF8.GetBytes(
                                File.ReadAllText(
                                    Path.Combine(Environment.CurrentDirectory, pvcbSecretsJson)
                                )
                            )
                        )
                    )
                    .Build()
                    .GetSection("Pvcb")
            );
            builder.Services.AddSingleton<IPvcbLocalization, PvcbLocalization>();
            builder.Services.AddSingleton<IPvcbMapErrorCode, PvcbMapErrorCode>();
            builder.Services.AddSingleton<IPvcbService, PvcbService>();
        }
    }
}
