using Business;

namespace WebApi
{
    public static class ServicesSetup
    {
        public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
        {
            // external libraries
            builder.Services.AddHttpContextAccessor();

            // scoped services
            builder.Services.AddScoped<IHealthChecker, HealthCheckService>();
            builder.Services.AddScoped<IReportProvider, ReportService>();

            builder.Services.AddScoped<IAuthenticator, AuthService>();
            builder.Services.AddScoped<IUserAuthenticator, UserAuthService>();
            builder.Services.AddScoped<IClientAuthenticator, ClientAuthService>();

            builder.Services.AddScoped<IJwtProvider, JwtService>();
            builder.Services.AddScoped<IOtpProvider, OtpService>();

            builder.Services.AddScoped<IClientProcessor, ClientService>();
            builder.Services.AddScoped<IUserProcessor, UserService>();

            return builder;
        }
    }
}