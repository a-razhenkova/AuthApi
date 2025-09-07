using Infrastructure;
using Infrastructure.Configuration.AppSettings;

namespace WebApi
{
    public static class ConfigurationSetup
    {
        public static WebApplicationBuilder BindConfigurationSource(this WebApplicationBuilder builder)
        {
            builder.Configuration.Sources.Clear();
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                 .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                                 .AddUserSecrets<Program>(optional: true, reloadOnChange: true);

            builder.Services.AddOptions<AppSettingsOptions>()
                .Bind(builder.Configuration)
                .ValidateDataAnnotations()
                .Validate(config =>
                {
                    config.Security.Validate();
                    config.Security.RateLimiter.Validate();
                    config.Security.AccessToken.Validate();
                    config.Security.MultiFactorAuth.Validate();

                    config.Database.Validate();
                    config.PaginatedReport.Validate();

                    config.InternalServices.Validate();
                    config.InternalServices.NotifyApi.Validate();
                    return true;
                })
                .ValidateOnStart();

            return builder;
        }

        public static TOptions GetRequiredSection<TOptions>(this IConfiguration config, string sectionName)
            where TOptions : new()
        {
            TOptions options = new();
            config.GetRequiredSection(sectionName).Bind(options);
            return options;
        }

        public static string GetRequiredConnectionString(this IConfiguration config, string connectionStringName)
            => config.GetConnectionString(ConnectionStringNames.Redis) ?? throw new ArgumentNullException();
    }
}