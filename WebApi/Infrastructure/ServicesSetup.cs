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
            builder.Services.AddScoped<IReportHandler, ReportService>();

            builder.Services.AddScoped<IClientHandler, ClientService>();
            builder.Services.AddScoped<IUserHandler, UserService>();

            builder.Services.AddScoped<IClientAuthenticator, ClientAuthenticationService>();
            builder.Services.AddScoped<IUserAuthenticator, UserAuthenticationService>();
            builder.Services.AddScoped<IBearerAuthenticator, BearerAuthenticationService>();
            builder.Services.AddScoped<IBasicAuthenticator, BasicAuthenticationService>();
            builder.Services.AddScoped<IOtpAuthenticator, OtpAuthenticationService>();

            builder.Services.AddScoped<ITokenHandler, TokenService>();

            return builder;
        }
    }
}