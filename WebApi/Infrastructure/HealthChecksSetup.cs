using Database.AuthDb;
using Infrastructure;
using Infrastructure.Configuration.AppSettings;

namespace WebApi
{
    public static class HealthChecksSetup
    {
        public static WebApplicationBuilder AddHealthChecks(this WebApplicationBuilder builder)
        {
            var databaseOptions = builder.Configuration.GetRequiredSection<DatabaseOptions>(nameof(AppSettingsOptions.Database));

            builder.Services.AddHealthChecks()
                            .AddDbContextCheck<AuthDbContext>("Authentication Database", tags: [HealthCheckImpactTag.Critical.ToString()])
                            .AddRedis(builder.Configuration.GetRequiredConnectionString(ConnectionStringNames.Redis), name: "Redis", tags: [HealthCheckImpactTag.Medium.ToString()]);

            return builder;
        }
    }
}