using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Elastic.CommonSchema;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using Serilog;
using Serilog.Core;
using Serilog.Formatting.Json;
using Serilog.Sinks.RabbitMQ;
using ShopDev.Constants.Environments;
using ShopDev.Constants.RabbitMQ;
using ShopDev.RabbitMQ.Configs;
using ShopDev.Utils.Security;
using ShopDev.WebAPIBase.Filters;
using ShopDev.WebAPIBase.Middlewares;
using StackExchange.Profiling.Storage;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace ShopDev.WebAPIBase
{
    /// <summary>
    /// Các hàm mở rộng cho program web api, cấu hình services, middleware pipeline
    /// </summary>
    public static class ProgramExtensions
    {
        public const string DbConnection = "Default";
        public const string Redis = "Redis";
        public const string Jwk = "Jwk";
        public const string CorsPolicy = "cors_policy";

        /// <summary>
        /// Config default services <br/>
        /// </summary>
        public static void ConfigureServices(this WebApplicationBuilder builder)
        {
            var services = builder.Services;
            services.AddOptions();
            services.AddHealthChecks();

            ConfigureAutoMap(services);

            services.AddHttpContextAccessor();
        }

        public static void ConfigureDistributedCacheRedis(this WebApplicationBuilder builder)
        {
            //string configStrings = builder.Configuration["RedisCache:Config"]!;
            ConfigurationOptions configOptions =
                new()
                {
                    ServiceName = "mymaster",
                    Password = "123qwe",
                    TieBreaker = "",
                };
            string? endpoint = builder.Configuration["RedisCache:RedisSentinel"];
            if (!string.IsNullOrEmpty(endpoint))
            {
                configOptions.EndPoints.Add("127.0.0.1", 26379);
            }
            ConnectionMultiplexer sentinelConnection = ConnectionMultiplexer.SentinelConnect(
                configOptions,
                Console.Out
            );
            var muxer = sentinelConnection.GetSentinelMasterConnection(configOptions);
            ConfigurationOptions masterConfig = new ConfigurationOptions
            {
                ServiceName = "mymaster",
                CommandMap = CommandMap.Default
            };
            var redisMasterConnection = sentinelConnection.GetSentinelMasterConnection(
                masterConfig,
                Console.Out
            );
            var s = redisMasterConnection.GetDatabase();
            //configOptions.CheckCertificateRevocation = false;
            //configOptions.CertificateValidation += (sender, cert, chain, errors) =>
            //{
            //    return true;
            //};
            //configOptions.CertificateSelection += delegate
            //{
            //    var path = Path.Combine(Directory.GetCurrentDirectory(), builder.Configuration["RedisCache:Ssl:CertPath"]!);
            //    var cert = new X509Certificate2(path);
            //    return cert;
            //};
            builder.Services.AddSingleton<IConnectionMultiplexer>(_ => muxer);
            //builder.Services.AddStackExchangeRedisCache(options =>
            //{
            //    options.ConfigurationOptions = configOptions;
            //});
        }

        public static void ConfigureSession(this WebApplicationBuilder builder)
        {
            ConfigurationManager configurationManager = builder.Configuration;
            builder.Services.AddSession(o =>
            {
                o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                o.Cookie.Name = configurationManager["Session:AuthCookieName"];
                o.Cookie.HttpOnly = true;
                o.IdleTimeout = TimeSpan.FromMinutes(30);
            });
        }

        public static void ConfigureLogging(
            this WebApplicationBuilder builder,
            string queueName,
            string routingKey
        )
        {
            var rabbitMqConfig = builder
                .Configuration.GetSection("RabbitMQ")
                .Get<RabbitMqConfig>()!;
            using IConnection rabbitMqConnection = rabbitMqConfig.CreateConnection();
            using IModel model = rabbitMqConnection.CreateModel();
            try
            {
                //Kiểm tra queue và exchange còn tồn tại hay không
                model.ExchangeDeclarePassive(RabbitExchangeNames.Log);
                model.QueueDeclarePassive(queueName);
            }
            catch
            {
                Dictionary<string, object> queueArgs = new() { { "x-queue-type", "quorum" } };
                model.QueueDeclare(
                    queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: queueArgs
                );
                model.ExchangeDeclare(
                    RabbitExchangeNames.Log,
                    ExchangeType.Direct,
                    durable: true,
                    autoDelete: false
                );
                model.QueueBind(queueName, RabbitExchangeNames.Log, routingKey);
            }

            RabbitMQClientConfiguration configRabbitMqSerilog =
                new()
                {
                    Username = rabbitMqConfig.Username,
                    Password = rabbitMqConfig.Password,
                    Port = rabbitMqConfig.Port,
                    VHost = rabbitMqConfig.VirtualHost ?? "/",
                    DeliveryMode = RabbitMQDeliveryMode.Durable,
                    Exchange = RabbitExchangeNames.Log,
                    ExchangeType = ExchangeType.Direct,
                    RouteKey = routingKey,
                };
            configRabbitMqSerilog.Hostnames.Add(rabbitMqConfig.HostName);

            if (rabbitMqConfig.Ssl is not null)
            {
                configRabbitMqSerilog.SslOption = new SslOption()
                {
                    ServerName = rabbitMqConfig.Ssl!.ServerName,
                    Enabled = true,
                    CertPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        rabbitMqConfig.Ssl!.CertPath!
                    ),
                    AcceptablePolicyErrors =
                        SslPolicyErrors.RemoteCertificateNameMismatch
                        | SslPolicyErrors.RemoteCertificateChainErrors
                };
            }

            var environment = builder.Environment.EnvironmentName;
            ConfigurationManager configurationManager = builder.Configuration;
            Logger logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .WriteTo.RabbitMQ(
                    (clientConfiguration, sinkConfiguration) =>
                    {
                        clientConfiguration.From(configRabbitMqSerilog);
                        sinkConfiguration.TextFormatter = new JsonFormatter();
                    }
                )
                .Enrich.WithProperty("Environment", environment)
                .ReadFrom.Configuration(configurationManager)
                .CreateLogger();
            builder.Host.UseSerilog(logger);
        }

        [Obsolete]
        public static void ConfigureSignalR(this WebApplicationBuilder builder)
        {
            //nếu có cấu hình redis
            string? redisConnectionString = builder.Configuration.GetConnectionString(Redis);

            //signalR
            var signalRBuilder = builder.Services.AddSignalR();
            if (!string.IsNullOrWhiteSpace(redisConnectionString))
            {
                signalRBuilder.AddStackExchangeRedis(
                    redisConnectionString,
                    options =>
                    {
                        options.Configuration.ChannelPrefix =
                            $"{{SignalR-{Assembly.GetExecutingAssembly().GetName().Name}}}";
                    }
                );
            }
        }

        public static void ConfigureAuthentication(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthorization();
            builder
                .Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(
                    JwtBearerDefaults.AuthenticationScheme,
                    options =>
                    {
                        var rsaSecurityKey = CryptographyUtils.ReadKey(
                            builder.Configuration.GetValue<string>("IdentityServer:PublicKey")!,
                            builder.Configuration.GetValue<string>("IdentityServer:PrivateKey")!
                        );

                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateAudience = false,
                            ValidateIssuer = false,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = rsaSecurityKey
                        };
                        options.RequireHttpsMetadata = false;

                        options.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = context =>
                            {
                                //lấy token trong header
                                var accessToken = context
                                    .Request.Query.FirstOrDefault(q => q.Key == "access_token")
                                    .Value.ToString();
                                if (string.IsNullOrEmpty(accessToken))
                                {
                                    accessToken = context
                                        .Request.Headers.FirstOrDefault(h =>
                                            h.Key == "access_token"
                                        )
                                        .Value.ToString();
                                }

                                // If the request is for our hub...
                                var path = context.HttpContext.Request.Path;
                                if (
                                    !string.IsNullOrEmpty(accessToken)
                                    && path.StartsWithSegments("/hub")
                                )
                                {
                                    // Read the token out of the query string
                                    context.Token = accessToken;
                                }
                                return Task.CompletedTask;
                            }
                        };
                    }
                );
        }

        public static void ConfigureMiniProfile(
            this WebApplicationBuilder builder,
            string miniProfilerBasePath
        )
        {
            builder
                .Services.AddMiniProfiler(options =>
                {
                    options.RouteBasePath = miniProfilerBasePath; //profiler/results-index
                    (options.Storage as MemoryCacheStorage)!.CacheDuration = TimeSpan.FromMinutes(
                        60
                    );
                    options.SqlFormatter =
                        new StackExchange.Profiling.SqlFormatters.InlineFormatter();
                })
                .AddEntityFramework();
        }

        public static void ConfigureSwagger(this WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen(option =>
            {
                option.OperationFilter<AddCommonParameterSwagger>();

                option.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Title = Assembly.GetEntryAssembly()?.GetName().Name,
                        Version = "v1"
                    }
                );

                option.AddSecurityDefinition(
                    JwtBearerDefaults.AuthenticationScheme,
                    new OpenApiSecurityScheme
                    {
                        Description =
                            "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                        Name = "Authorization",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey
                    }
                );

                option.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = JwtBearerDefaults.AuthenticationScheme
                                }
                            },
                            Array.Empty<string>()
                        }
                    }
                );

                // Set the comments path for the Swagger JSON and UI.**
                var xmlFile = Path.Combine(
                    AppContext.BaseDirectory,
                    $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"
                );
                if (System.IO.File.Exists(xmlFile))
                {
                    option.IncludeXmlComments(xmlFile);
                }
                var projectDependencies = Assembly
                    .GetEntryAssembly()!
                    .CustomAttributes.SelectMany(c =>
                        c.ConstructorArguments.Select(ca => ca.Value?.ToString())
                    )
                    .Where(o => o != null)
                    .ToList();
                foreach (var assembly in projectDependencies)
                {
                    var otherXml = Path.Combine(AppContext.BaseDirectory, $"{assembly}.xml");
                    if (System.IO.File.Exists(otherXml))
                    {
                        option.IncludeXmlComments(otherXml);
                    }
                }
                option.CustomSchemaIds(x => x.FullName);
            });
        }

        /// <summary>
        /// Config default middleware pipeline
        /// </summary>
        public static void Configure(this WebApplication app)
        {
            if (EnvironmentNames.DevelopEnv.Contains(app.Environment.EnvironmentName))
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint(
                        "/swagger/v1/swagger.json",
                        $"{Assembly.GetExecutingAssembly().GetName().Name} v1"
                    );
                    options.DocExpansion(DocExpansion.None);
                });
            }
            //app.UseHttpsRedirection();

            app.UseMiniProfiler();
            app.UseRequestLocalizationCustom();
            app.UseForwardedHeaders();
            app.UseRouting();
            app.UseCors(CorsPolicy);

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
        }

        public static void UseSwaggerConfig(this WebApplication app, string prefixPath)
        {
            app.UseSwagger(option =>
            {
                option.RouteTemplate = $"{prefixPath}/{{documentName}}/swagger.json";
            });
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint(
                    $"/{prefixPath}/v1/swagger.json",
                    $"{Assembly.GetEntryAssembly()?.GetName().Name} v1"
                );
                options.RoutePrefix = prefixPath;
                options.DocExpansion(DocExpansion.None);
            });
        }

        public static void ConfigureEndpoint(this WebApplication app)
        {
            app.UseHangfireDashboard();
            app.MapHealthChecks("/health");
            app.MapHangfireDashboard("/hangfire");
            app.MapControllers();
        }

        /// <summary>
        /// Kiểm tra resolve service
        /// </summary>
        /// <param name="app"></param>
        /// <param name="services"></param>
        public static void TestResolveService(this WebApplication app, IServiceCollection services)
        {
            using var scope = app.Services.CreateScope();
            foreach (var service in services)
            {
                var type = service.ServiceType;
                if (!type.FullName!.Contains("Service"))
                {
                    continue;
                }
                app.Services.GetService(type);
            }
        }

        /// <summary>
        /// Tự tìm trong các Project Dependencies xem có định nghĩa class AutoMapProfile nào không nếu có thì thêm vào hàm AddAutoMapper
        /// </summary>
        public static void ConfigureAutoMap(IServiceCollection services)
        {
            List<string> projectDependencyNames =
            [
                .. Assembly.GetEntryAssembly()?.CustomAttributes
                                                                        .SelectMany(c => c.ConstructorArguments.Select(ca => ca.Value?.ToString()))
                                                                        .Where(o => o != null)
                                                                        .ToList(),
                .. Assembly.GetExecutingAssembly().CustomAttributes
                                                                        .SelectMany(c => c.ConstructorArguments.Select(ca => ca.Value?.ToString()))
                                                                        .Where(o => o != null)
                                                                        .ToList(),
                .. Assembly.GetCallingAssembly().CustomAttributes
                                                                        .SelectMany(c => c.ConstructorArguments.Select(ca => ca.Value?.ToString()))
                                                                        .Where(o => o != null)
                                                                        .ToList(),
            ];
            List<Type> autoMapProfiles = new();

            foreach (var dependency in projectDependencyNames)
            {
                Assembly? assembly = null;
                //thử load assembly
                try
                {
                    assembly = Assembly.Load(dependency);
                }
                catch { }

                if (assembly != null)
                {
                    var getAutoMapProfiles = assembly
                        .DefinedTypes.Where(dt => dt.BaseType?.FullName == "AutoMapper.Profile")
                        .Select(o => o.AsType())
                        .ToList();
                    autoMapProfiles.AddRange(getAutoMapProfiles);
                }
            }

            services.AddAutoMapper(autoMapProfiles.ToArray());
        }

        public static void ConfigureCors(this WebApplicationBuilder builder)
        {
            string allowOrigins = builder.Configuration.GetSection("AllowedOrigins")!.Value!;
            //File.WriteAllText("cors.now.txt", $"CORS: {allowOrigins}");
            var origins = allowOrigins
                .Split(';')
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToArray();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    CorsPolicy,
                    builder =>
                    {
                        builder
                            .WithOrigins(origins)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials()
                            .WithExposedHeaders("Content-Disposition");
                    }
                );
            });
        }

        public static void ConfigureHangfire(
            this WebApplicationBuilder builder,
            string connectionString,
            string schemaName
        )
        {
            builder.Services.AddHangfire(configuration =>
                configuration
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSerilogLogProvider()
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSqlServerStorage(
                        connectionString,
                        new SqlServerStorageOptions { SchemaName = schemaName, }
                    )
            );

            JobStorage.Current = new SqlServerStorage(
                connectionString,
                new SqlServerStorageOptions { SchemaName = schemaName, }
            );

            builder.Services.AddHangfireServer(
                (service, options) =>
                {
                    options.ServerName = Assembly.GetEntryAssembly()?.GetName().Name;
                    options.WorkerCount = 200;
                }
            );
        }

        public static void UpdateMigrations<TDbContext>(this WebApplication app)
            where TDbContext : DbContext
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TDbContext>();
            if (db.Database.GetPendingMigrations().Any())
            {
                db.Database.Migrate();
            }
        }

        public static void ConfigureOpenIddict<TDbContext>(this WebApplicationBuilder builder)
            where TDbContext : DbContext
        {
            builder
                .Services.AddOpenIddict()
                // Register the OpenIddict core components.
                .AddCore(options =>
                {
                    // Configure OpenIddict to use the Entity Framework Core stores and models.
                    // Note: call ReplaceDefaultEntities() to replace the default entities.
                    options.UseEntityFrameworkCore().UseDbContext<TDbContext>();
                });
        }

        public static void ConfigureDataProtection(this WebApplicationBuilder builder)
        {
            var protectKeyCer = builder
                .Configuration.GetSection("IdentityServer:ProtectKeyCer")
                .Value;
            X509Certificate2 certificate = new(System.IO.File.ReadAllBytes(protectKeyCer!));
            builder
                .Services.AddDataProtection()
                .SetApplicationName("ShopDev")
                .ProtectKeysWithCertificate(certificate);
        }
    }
}
