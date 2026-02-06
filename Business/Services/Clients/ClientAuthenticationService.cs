using Database.IdentityDb;
using Database.IdentityDb.DefaultSchema;
using Infrastructure;
using Infrastructure.Configuration.AppSettings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Business
{
    public class ClientAuthenticationService : IClientAuthenticator
    {
        private readonly AppSettingsOptions _appSettingsOptions;
        private readonly IdentityDbContext _identityDbContext;

        public ClientAuthenticationService(IOptionsSnapshot<AppSettingsOptions> appSettingsOptions,
                                          IdentityDbContext identityDbContext)
        {
            _appSettingsOptions = appSettingsOptions.Value;
            _identityDbContext = identityDbContext;
        }

        public async Task<Client> AuthenticateAsync(string key)
        {
            Client client = await _identityDbContext.Client
                .Where(c => c.Key == key)
                .Include(c => c.Status)
                .Include(c => c.Right)
                .Include(c => c.Subscriptions.Where(s => s.Subscription.ExpirationDate >= DateTime.UtcNow.Date))
                .SingleOrDefaultAsync() ?? throw new UnauthorizedException("Invalid credentials.");

            CheckClientStatus(client.Status);

            if (!client.IsInternal && !client.Subscriptions.Any())
            {
                client.Block(ClientStatusReasons.ExpiredSubscription);
                await _identityDbContext.SaveChangesAsync();

                throw new ForbiddenException("Client subscription has expired.");
            }

            return client;
        }

        public async Task<Client> AuthenticateAsync(string key, string secret)
        {
            Client client = await AuthenticateAsync(key);

            bool isSecretValid = client.IsSecretValid(secret);

            await ProcessLoginAttempt(client, isSecretValid);

            if (!isSecretValid)
                throw new UnauthorizedException("Invalid credentials.");

            return client;
        }

        private void CheckClientStatus(ClientStatus status)
        {
            if (status.Value == ClientStatuses.Blocked
             || status.Value == ClientStatuses.Disabled)
            {
                throw new ForbiddenException($"Client status is '{status.Value}'.");
            }
        }

        private async Task ProcessLoginAttempt(Client client, bool isSecretValid)
        {
            if (isSecretValid)
            {
                client.WrongLoginAttemptsCounter = 0;
            }
            else
            {
                client.WrongLoginAttemptsCounter++;

                if (client.WrongLoginAttemptsCounter >= _appSettingsOptions.Security.DefaultMaxWrongLoginAttemptsBeforeBlock)
                    client.Block(ClientStatusReasons.MaxWrongLoginAttemptsReached);
            }

            await _identityDbContext.SaveChangesAsync();
        }
    }
}