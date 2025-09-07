using Infrastructure.Configuration.AppSettings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;

namespace WebApi.Tests
{
    public sealed class AppSettingsOptionsMock
    {
        private static IOptions<AppSettingsOptions>? _appSettingsOptions;
        private static IOptionsMonitor<AppSettingsOptions>? _appSettingsIOptionsMonitor;
        private static IOptionsSnapshot<AppSettingsOptions>? _appSettingsIOptionsSnapshot;

        private AppSettingsOptionsMock()
        {

        }

        public static IOptions<AppSettingsOptions> GetOptions()
        {
            if (_appSettingsOptions is null)
            {
                AppSettingsOptions appSettingsOptions = LoadAppSettingsOptions();

                var options = new Mock<IOptions<AppSettingsOptions>>();
                options.Setup(o => o.Value).Returns(appSettingsOptions);

                _appSettingsOptions = options.Object;
            }

            return _appSettingsOptions;
        }

        public static IOptionsMonitor<AppSettingsOptions> GetOptionsMonitor()
        {
            if (_appSettingsIOptionsMonitor is null)
            {
                AppSettingsOptions appSettingsOptions = LoadAppSettingsOptions();

                var options = new Mock<IOptionsMonitor<AppSettingsOptions>>();
                options.Setup(o => o.CurrentValue).Returns(appSettingsOptions);

                _appSettingsIOptionsMonitor = options.Object;
            }

            return _appSettingsIOptionsMonitor;
        }

        public static IOptionsSnapshot<AppSettingsOptions> GetSnapshot()
        {
            if (_appSettingsIOptionsSnapshot is null)
            {
                AppSettingsOptions appSettingsOptions = LoadAppSettingsOptions();

                var options = new Mock<IOptionsSnapshot<AppSettingsOptions>>();
                options.Setup(o => o.Value).Returns(appSettingsOptions);

                _appSettingsIOptionsSnapshot = options.Object;
            }

            return _appSettingsIOptionsSnapshot;
        }

        private static AppSettingsOptions LoadAppSettingsOptions()
        {
            var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddUserSecrets("f60b3780-88fb-473d-b39f-2aee10790fbc")
                    .Build();

            var appSettingsOptions = new AppSettingsOptions();
            configuration.Bind(appSettingsOptions);

            return appSettingsOptions;
        }
    }
}