using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using ShopDev.ApplicationBase.Localization;
using ShopDev.Authentication.API.HostedServices;
using ShopDev.Authentication.API.Models;
using ShopDev.Authentication.ApplicationServices.AuthenticationModule.Abstract;
using ShopDev.Authentication.ApplicationServices.AuthenticationModule.Implements;
using ShopDev.Authentication.ApplicationServices.Common;
using ShopDev.Authentication.ApplicationServices.Common.Localization;
using ShopDev.Authentication.ApplicationServices.Middlewares;
using ShopDev.Authentication.Infrastructure.Persistence;
using ShopDev.Common.Filters;
using ShopDev.Constants.Authorization;
using ShopDev.Constants.Common;
using ShopDev.Constants.Database;
using ShopDev.Constants.Environments;
using ShopDev.Constants.RabbitMQ;
using ShopDev.IdentityServerBase.Middlewares;
using ShopDev.IdentityServerBase.StartUp;
using ShopDev.S3Bucket;
using ShopDev.S3Bucket.Configs;
using ShopDev.WebAPIBase;
using ShopDev.WebAPIBase.Filters;
using ShopDev.WebAPIBase.Middlewares;

namespace ShopDev.Authentication.API
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.ConfigureLogging(RabbitQueues.LogAuth, RabbitRoutingKeys.LogAuth);
            builder.ConfigureServices();
            builder.ConfigureDataProtection();

            builder
                .Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(c =>
                {
                    c.LoginPath = AuthenticationPath.AuthenticateLogin;
                });
            builder.Services.Configure<FormOptions>(o =>
            {
                o.MultipartBodyLengthLimit = int.MaxValue; //tối đa 2GB
            });

            builder
                .Services.AddControllersWithViews(options =>
                {
                    options.Filters.Add<ExceptionFilter>();
                    options.Filters.Add<CustomValidationErrorAttribute>();
                })
                .AddRazorRuntimeCompilation();

            builder.ConfigureSwagger();
            builder.ConfigureAuthentication();
            builder.ConfigureCors();

            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.AddSlidingWindowLimiter(
                    policyName: LimitRatePolicyName.LimitLogin,
                    options =>
                    {
                        options.PermitLimit = 30;
                        options.Window = TimeSpan.FromSeconds(60);
                        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                        options.SegmentsPerWindow = 30;
                        options.QueueLimit = 0; //bỏ qua
                    }
                );
                options.AddSlidingWindowLimiter(
                    policyName: LimitRatePolicyName.LimitRegister,
                    options =>
                    {
                        options.PermitLimit = 30;
                        options.Window = TimeSpan.FromSeconds(60);
                        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                        options.SegmentsPerWindow = 30;
                        options.QueueLimit = 0; //bỏ qua
                    }
                );
            });

            string connectionString =
                builder.Configuration.GetConnectionString("Default")
                ?? throw new InvalidOperationException(
                    "Không tìm thấy connection string \"Default\" trong appsettings.json"
                );
            Console.WriteLine(connectionString);

            //entity framework
            builder.Services.AddDbContextPool<AuthenticationDbContext>(
                options =>
                {
                    //options.UseInMemoryDatabase("DbDefault");
                    options.UseSqlServer(
                        connectionString,
                        options =>
                        {
                            options.MigrationsAssembly(typeof(Program).Namespace);
                            options.MigrationsHistoryTable(
                                DbSchemas.TableMigrationsHistory,
                                DbSchemas.Default
                            );
                        }
                    );
                    options.UseOpenIddict();
                },
                poolSize: 128
            );

            builder.ConfigureS3();
            builder.Services.Configure<AuthWorkerConfiguration>(
                builder.Configuration.GetSection("IdentityServer:Clients")
            );
            builder.Services.AddCommonIdentityServer<AuthenticationDbContext, ShopDevAuthWorker>(
                builder.Configuration
            );

            builder.Services.AddSingleton<IMapErrorCode, AuthenticationMapErrorCode>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IPermissionService, PermissionService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<INotificationTokenService, NotificationTokenService>();
            builder.Services.AddScoped<IS3ManagerFile, S3ManagerFile>();
            builder.Services.AddSingleton<LocalizationBase, AuthenticationLocalization>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (EnvironmentNames.DevelopEnv.Any(x => x == app.Environment.EnvironmentName))
            {
                Console.WriteLine(app.Environment.EnvironmentName);
                app.UseDeveloperExceptionPage();
                app.UseSwaggerConfig("api/auth/swagger");
            }

            if (EnvironmentNames.Productions.Any(x => x == app.Environment.EnvironmentName))
            {
                app.UseHttpsRedirection();
                app.UpdateMigrations<AuthenticationDbContext>();
            }
            app.UseRateLimiter();
            app.UseCors(ProgramExtensions.CorsPolicy);
            app.UseForwardedHeaders();
            app.UseStaticFiles();
            app.UseRequestLocalizationCustom();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCheckAuthorizationToken();
            //app.UseCheckUser();
            app.MapControllers();
            app.MapHealthChecks("/health");
            app.Run();
        }
    }
}
