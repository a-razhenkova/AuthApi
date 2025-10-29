using Infrastructure;
using Infrastructure.Configuration.AppSettings;

namespace WebApi
{
    public static class ConfigurationSetup
    {
        public static WebApplicationBuilder BindConfigurationSources(this WebApplicationBuilder builder)
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
                    config.Log.Validate();
                    config.Log.File.Validate();

                    config.Security.Validate();
                    config.Security.RateLimiter.Validate();
                    config.Security.AccessToken.Validate();
                    config.Security.RefreshToken.Validate();
                    config.Security.MultiFactorAuth.Validate();

                    config.Database.Validate();
                    config.PaginatedReport.Validate();

                    config.ExternalServices.Validate();
                    config.ExternalServices.NotifyApi.Validate();
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
            => config.GetConnectionString(connectionStringName) ?? throw new ArgumentNullException();
    }
}