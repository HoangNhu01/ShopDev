using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ShopDev.Authentication.Infrastructure.Persistence;
using ShopDev.Common.Filters;
using ShopDev.Constants.Environments;
using ShopDev.RabbitMQ.Configs;
using ShopDev.WebAPIBase;
using ShopDev.WebAPIBase.Filters;
using ShopDev.WebAPIBase.Middlewares;

namespace ShopDev.Logistic.API
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
					options.UseSqlServer(authConnectionString);
					options.UseOpenIddict();
				},
				poolSize: 128
			);
			string logisticConnectionString =
				builder.Configuration.GetConnectionString("Default")
				?? throw new InvalidOperationException(
					"Không tìm thấy connection string \"Default\" trong appsettings.json"
				);

			//builder.Services.AddDbContextPool<AuthenticationDbContext>(
			//    options =>
			//    {
			//        //options.UseInMemoryDatabase("DbDefault");
			//        options.UseSqlServer(
			//            logisticConnectionString,
			//            options =>
			//            {
			//                options.MigrationsAssembly(typeof(Program).Namespace);
			//                options.MigrationsHistoryTable(
			//                    DbSchemas.TableMigrationsHistory,
			//                    DbSchemas.Default
			//                );
			//            }
			//        );
			//        options.UseOpenIddict();
			//    },
			//    poolSize: 128
			//);
			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (EnvironmentNames.DevelopEnv.Contains(app.Environment.EnvironmentName))
			{
				app.UseDeveloperExceptionPage();
				app.UseSwaggerConfig("api/logistic/swagger");
			}

			if (EnvironmentNames.Productions.Contains(app.Environment.EnvironmentName))
			{
				app.UseHttpsRedirection();
				app.UpdateMigrations<AuthenticationDbContext>();
			}
			app.UseCors(ProgramExtensions.CorsPolicy);
			app.UseForwardedHeaders();
			app.UseStaticFiles();
			app.UseRequestLocalizationCustom();
			app.UseAuthentication();
			app.UseAuthorization();
			//app.UseCheckAuthorizationToken();
			//app.UseCheckUser();
			app.MapControllers();
			app.MapHealthChecks("/health");
			app.Run();
		}
	}
}
