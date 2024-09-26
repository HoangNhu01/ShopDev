using System.Net;
using Consul;
using Hangfire;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ShopDev.ApplicationBase.Localization;
using ShopDev.Authentication.Infrastructure.Persistence;
using ShopDev.Common.Filters;
using ShopDev.Constants.Database;
using ShopDev.Constants.Environments;
using ShopDev.Order.API.HostedServices;
using ShopDev.Order.ApplicationServices.CartModule.Abstract;
using ShopDev.Order.ApplicationServices.CartModule.Implements;
using ShopDev.Order.ApplicationServices.Choreography.Consumers.Abstracts;
using ShopDev.Order.ApplicationServices.Choreography.Consumers.Implememts;
using ShopDev.Order.ApplicationServices.Choreography.Producers.Abstracts;
using ShopDev.Order.ApplicationServices.Choreography.Producers.Implememts;
using ShopDev.Order.ApplicationServices.Common;
using ShopDev.Order.ApplicationServices.Common.Localization;
using ShopDev.Order.ApplicationServices.OrderModule.Abstracts;
using ShopDev.Order.ApplicationServices.OrderModule.Implements;
using ShopDev.Order.Infrastructure.Persistence;
using ShopDev.PaymentTool.Configs;
using ShopDev.RabbitMQ.Configs;
using ShopDev.ServiceDiscovery.Config;
using ShopDev.ServiceDiscovery.Configs;
using ShopDev.WebAPIBase;
using ShopDev.WebAPIBase.Filters;
using ShopDev.WebAPIBase.Middlewares;

namespace ShopDev.Order.API
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.ConfigureServices();
            builder.ConfigureDataProtection();

            builder.Services.AddControllers();
            builder
                .Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();
            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add<ExceptionFilter>();
                options.Filters.Add<CustomValidationErrorAttribute>();
            });
            builder.ConfigureSwagger();
            builder.ConfigureAuthentication();
            builder.ConfigureCors();
            builder.ConfigureRabbitMQ();
            builder.ConfigureDistributedCacheRedis();
            builder.ServiceDiscovery();

            builder.ConfigurePaymentTool();
            // Khởi tạo instance cho MongoDB
            builder.Services.AddSingleton<IMapErrorCode, OrderMapErrorCode>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddSingleton<LocalizationBase, OrderLocalization>();
            builder.Services.AddSingleton<IUpdateStockProducer, UpdateStockProducer>();
            builder.Services.AddSingleton<IUpdateOrderConsumer, UpdateOrderConsumer>();
            builder.Services.AddHostedService<ConsumerHostedService>();
            string authConnectionString =
                builder.Configuration.GetConnectionString("Default")
                ?? throw new InvalidOperationException(
                    "Không tìm thấy connection string \"Default\" trong appsettings.json"
                );

            string namespaceOfProgram =
                typeof(Program).Namespace
                ?? throw new InvalidOperationException("Không lấy được namespace của Program");

            builder.Services.AddDbContextPool<AuthenticationDbContext>(
                options =>
                {
                    options.UseSqlServer(
                        authConnectionString,
                        sqlOptions =>
                        {
                            sqlOptions.EnableRetryOnFailure();
                        }
                    );
                    options.UseOpenIddict();
                },
                poolSize: 128
            );
            string orderConnectionString =
                builder.Configuration.GetConnectionString("OrderDb")
                ?? throw new InvalidOperationException(
                    "Không tìm thấy connection string \"OrderDb\" trong appsettings.json"
                );
            Console.WriteLine(orderConnectionString);
            builder.Services.AddDbContextPool<OrderDbContext>(
                options =>
                {
                    //options.UseInMemoryDatabase("DbDefault");
                    options.UseSqlServer(
                        orderConnectionString,
                        sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(typeof(Program).Namespace);
                            sqlOptions.MigrationsHistoryTable(
                                DbSchemas.TableMigrationsHistory,
                                DbSchemas.SDOrder
                            );
                            sqlOptions.EnableRetryOnFailure();
                        }
                    );
                    options.UseLazyLoadingProxies();
                },
                poolSize: 128
            );
            builder.ConfigureHangfire(orderConnectionString, DbSchemas.SDOrder);
            builder.Services.Configure<ConsulConfig>(
                builder.Configuration.GetSection("Consul:OrderService")
            );

            var app = builder.Build();
            //using (var scope = app.Services.CreateScope())
            //{
            //    var migration = scope.ServiceProvider.GetRequiredService<ExtensionsMigration>();
            //    migration.CreateNewCollectionAsync().Wait();
            //}
            // Configure the HTTP request pipeline.
            if (EnvironmentNames.DevelopEnv.Contains(app.Environment.EnvironmentName))
            {
                app.UseDeveloperExceptionPage();
                app.UseSwaggerConfig("api/order/swagger");
            }

            using (var scope = app.Services.CreateScope())
            {
                IOrderService orderService =
                    scope.ServiceProvider.GetRequiredService<IOrderService>();
                RecurringJob.AddOrUpdate(
                    nameof(orderService.ExecuteUpdateStock),
                    () => orderService.ExecuteUpdateStock(null),
                    "*/30 * * * * *",
                    new RecurringJobOptions
                    {
                        TimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"),
                    }
                );
            }
            if (EnvironmentNames.Productions.Contains(app.Environment.EnvironmentName))
            {
                app.UseHttpsRedirection();
                //app.UpdateMigrations<AuthenticationDbContext>();
            }
            app.UseCors(ProgramExtensions.CorsPolicy);
            app.UseForwardedHeaders();
            app.UseStaticFiles();
            app.UseRequestLocalizationCustom();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHangfireDashboard(
                "/api/order/hangfire"
            //,
            //new DashboardOptions { Authorization = new[] { new HangfireAuthorizationFilter() } }
            );
            //app.UseCheckAuthorizationToken();
            //app.UseCheckUser();
            app.MapControllers();
            app.MapHealthChecks("/health");
            app.Run();
        }
    }
}
