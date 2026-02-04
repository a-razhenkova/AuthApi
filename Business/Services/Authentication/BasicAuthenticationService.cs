using Database.AuthDb;
using Database.AuthDb.DefaultSchema;
using Infrastructure;
using Infrastructure.Configuration.AppSettings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Business
{
    public class BasicAuthenticationService : IBasicAuthenticator
    {
        private readonly AppSettingsOptions _appSettingsOptions;
        private readonly AuthDbContext _authDbContext;

        public BasicAuthenticationService(IOptionsSnapshot<AppSettingsOptions> appSettingsOptions,
                                         AuthDbContext authDbContext)
        {
            _appSettingsOptions = appSettingsOptions.Value;
            _authDbContext = authDbContext;
        }

        public async Task<Client> AuthenticateAsync(Authorization authorization)
        {
            if (authorization.Schema != AuthorizationSchema.Basic)
                throw new BadRequestException("Invalid token format.");

            var credentials = Utils.DecodeBasicAuthCredentials(authorization.Value);

            if (!ClientKey.IsValid(credentials.Key) || !ClientSecret.IsValid(credentials.Secret))
                throw new UnauthorizedException("Invalid credentials.");

            return await AuthenticateAsync(credentials.Key, credentials.Secret);
        }

        public async Task<Client> AuthenticateAsync(string clientKey, string clientSecret)
        {
            Client client = await _authDbContext.Client
                .Where(c => c.Key == clientKey)
                .Include(c => c.Status)
                .Include(c => c.Right)
                .Include(c => c.Subscriptions.Where(s => s.Subscription.ExpirationDate >= DateTime.UtcNow.Date))
                .SingleOrDefaultAsync() ?? throw new UnauthorizedException("Invalid credentials.");

            if (!client.Status.IsAuthAllowed())
                throw new ForbiddenException($"Client status is '{client.Status.Value}'.");

            if (!client.IsInternal && !client.Subscriptions.Any())
            {
                client.Status.Value = ClientStatuses.Disabled;
                client.Status.Reason = ClientStatusReasons.ExpiredSubscription;
                await _authDbContext.SaveChangesAsync();

                throw new ForbiddenException("Client subscription has expired.");
            }

            bool isSecretValid = client.Secret == clientSecret;

            AddLoginAttemptAsync(client, isSecretValid);

            if (!isSecretValid)
                throw new UnauthorizedException("Invalid credentials.");

            return client;
        }

        private void AddLoginAttemptAsync(Client client, bool isSuccessful)
        {
            if (isSuccessful)
            {
                client.WrongLoginAttemptsCounter = 0;
            }
            else
            {
                client.WrongLoginAttemptsCounter++;

                if (client.WrongLoginAttemptsCounter >= _appSettingsOptions.Security.DefaultMaxWrongLoginAttemptsBeforeBlock)
                {
                    client.Status.Value = ClientStatuses.Blocked;
                    client.Status.Reason = ClientStatusReasons.MaxWrongLoginAttemptsReached;
                }
            }
        }
    }
}