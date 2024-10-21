using MB.SignalR.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace MB.SignalR.Configs
{
	public static class SignalRConfigStartUp
	{
		public static void ConfigureSignalR(this WebApplicationBuilder builder)
		{
			string configStrings = builder.Configuration["RedisCache:Config"]!;
			ConfigurationOptions configOptions = ConfigurationOptions.Parse(configStrings);
			configOptions.CheckCertificateRevocation = false;
			configOptions.CertificateValidation += (sender, cert, chain, errors) =>
			{
				return true;
			};
			configOptions.CertificateSelection += delegate
			{
				var path = Path.Combine(
					Directory.GetCurrentDirectory(),
					builder.Configuration["RedisCache:Ssl:CertPath"]!
				);
				var cert = new X509Certificate2(path);
				return cert;
			};
			configOptions.ChannelPrefix = RedisChannel.Literal(
				$"{{SignalR-{Assembly.GetExecutingAssembly().GetName().Name}}}"
			);

			//signalR
			var signalRBuilder = builder.Services.AddSignalR();
			signalRBuilder.AddStackExchangeRedis(
				(o) =>
				{
					o.ConnectionFactory = async writer =>
					{
						var config = configOptions;
						var multiplexer = builder
							.Services.BuildServiceProvider()
							.GetRequiredService<IConnectionMultiplexer>();

						var connection = multiplexer;

						connection.ConnectionFailed += (_, e) =>
						{
							Console.WriteLine("Connection to Redis failed.");
						};

						if (!connection.IsConnected)
						{
							Console.WriteLine("Did not connect to Redis.");
						}

						return connection;
					};
				}
			);
		}
	}
}
