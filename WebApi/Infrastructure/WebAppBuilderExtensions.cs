using AutoMapper;
using Business;
using Database;
using Database.AuthDb;
using Infrastructure;
using Infrastructure.Configuration.AppSettings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

namespace WebApi
{
    public static class WebAppBuilderExtensions
    {
        public static WebApplicationBuilder AddHealthChecks(this WebApplicationBuilder builder)
        {
            var databaseOptions = builder.Configuration.GetRequiredSection<DatabaseOptions>(nameof(AppSettingsOptions.Database));

            builder.Services.AddHealthChecks()
                            .AddDbContextCheck<AuthDbContext>("Authentication Database", tags: [HealthCheckImpactTag.Critical.ToString()])
                            .AddRedis(builder.Configuration.GetRequiredConnectionString(ConnectionStringNames.Redis), name: "Redis", tags: [HealthCheckImpactTag.Medium.ToString()]);

            return builder;
        }

        public static MapperConfiguration CreateMapperConfig()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AuthDbMapperProfile());

                cfg.AddProfile(new V1.CommonMapperProfile());
                cfg.AddProfile(new V2.CommonMapperProfile());
            });

            mapperConfig.AssertConfigurationIsValid();

            return mapperConfig;
        }

        public static WebApplicationBuilder AddMapper(this WebApplicationBuilder builder)
        {
            MapperConfiguration mapperConfig = CreateMapperConfig();

            IMapper mapper = mapperConfig.CreateMapper();
            builder.Services.AddSingleton(mapper);

            return builder;
        }

        public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
        {
            DatabaseAttribute authDbConfig = typeof(AuthDbContext).GetRequiredCustomAttribute<DatabaseAttribute>();
            builder.Services.AddDbContext<AuthDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetRequiredConnectionString(authDbConfig.ConnectionStringName), cfg =>
                {
                    cfg.CommandTimeout(authDbConfig.CommandTimeoutInSeconds);
                    cfg.MigrationsAssembly(DatabaseAssembly.GetExecutingAssembly());
                    cfg.MigrationsHistoryTable(authDbConfig.MigrationsHistoryTableName, authDbConfig.DefaultSchemaName);
                });
#if DEBUG
                opt.LogTo(src => Debug.WriteLine(src));
                opt.EnableDetailedErrors();
                opt.EnableSensitiveDataLogging();
#endif
            });

            return builder;
        }

        public static WebApplicationBuilder AddCache(this WebApplicationBuilder builder)
        {
            builder.Services.AddStackExchangeRedisCache(opt => opt.Configuration = builder.Configuration.GetRequiredConnectionString(ConnectionStringNames.Redis));
            builder.Services.AddScoped<IRedisProvider, RedisProxy>();
            return builder;
        }

        public static WebApplicationBuilder AddAuthentication(this WebApplicationBuilder builder)
        {
            var securityOptions = builder.Configuration.GetRequiredSection<SecurityOptions>(nameof(AppSettingsOptions.Security));

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                            .AddJwtBearer(opt =>
                            {
                                opt.SaveToken = true;
                                opt.TokenValidationParameters = JwtService.GetAccessValidationParameters(securityOptions);
                            });

            return builder;
        }

        public static WebApplicationBuilder AddAuthorization(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IAuthorizationHandler, UserAuthorizationHandler>();
            builder.Services.AddAuthorization();
            return builder;
        }

        public static WebApplicationBuilder AddRateLimiter(this WebApplicationBuilder builder)
        {
            var securityOptions = builder.Configuration.GetRequiredSection<SecurityOptions>(nameof(AppSettingsOptions.Security));

            builder.Services.AddRateLimiter(opt =>
            {
                opt.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                opt.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    string? userIp = httpContext.GetUserIpAddress();

                    return RateLimitPartition.GetFixedWindowLimiter(!string.IsNullOrWhiteSpace(userIp) ? userIp : httpContext.Request.Headers.Host.ToString(),
                    partition => new FixedWindowRateLimiterOptions()
                    {
                        Window = TimeSpan.FromSeconds(securityOptions.RateLimiter.WindowInSeconds),
                        AutoReplenishment = true,
                        PermitLimit = securityOptions.RateLimiter.RequestsPerWindow,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    });
                });
            });

            return builder;
        }

        public static WebApplicationBuilder AddControllers(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers()
                            .AddJsonOptions(opt =>
                            {
                                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseUpper));
                            })
                            .ConfigureApiBehaviorOptions(opt =>
                            {
                                opt.InvalidModelStateResponseFactory = actionContext => CreateInvalidModelResponse(actionContext);
                            });
            builder.Services.AddRouting(opt => opt.LowercaseUrls = true);

            return builder;
        }

        private static IActionResult CreateInvalidModelResponse(ActionContext context)
        {
            var message = new StringBuilder();

            foreach (KeyValuePair<string, ModelStateEntry> entry in context.ModelState)
            {
                if (message.Length > 0)
                    message.AppendLine();

                foreach (var error in entry.Value.Errors)
                    message.Append(error.ErrorMessage);
            }

            throw new BadRequestException(new Exception(message.ToString()));
        }
    }
}