using Database.IdentityDb;
using Database.IdentityDb.DefaultSchema;
using Infrastructure;
using Infrastructure.Configuration.AppSettings;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Business
{
    public class UserAuthenticationService : IUserAuthenticator
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppSettingsOptions _appSettingsOptions;
        private readonly IdentityDbContext _identityDbContext;

        public UserAuthenticationService(IHttpContextAccessor httpContextAccessor,
                                        IOptionsSnapshot<AppSettingsOptions> appSettingsOptions,
                                        IdentityDbContext identityDbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _appSettingsOptions = appSettingsOptions.Value;
            _identityDbContext = identityDbContext;
        }

        public async Task<User> AuthenticateAsync(string externalId)
        {
            User user = await _identityDbContext.User
               .Where(u => u.ExternalId == externalId)
               .Include(u => u.Status)
               .Include(u => u.Login)
               .SingleOrDefaultAsync() ?? throw new UnauthorizedException();

            CheckUserStatus(user.Status);

            return user;
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            User user = await _identityDbContext.User
                .Where(u => u.Username == username)
                .Include(u => u.Status)
                .Include(u => u.Password)
                .Include(u => u.Login)
                .SingleOrDefaultAsync() ?? throw new UnauthorizedException("Invalid credentials.");

            CheckUserStatus(user.Status);

            bool isPasswordValid = UserSecurePassword.IsValid(user.Password.Value, password, user.Password.Secret);

            user.Login ??= new Login();
            string? lastLoginIpAddress = user.Login.LastLoginIpAddress;

            await ProcessLoginAttempt(user, isPasswordValid);

            if (user.Status.Value == UserStatuses.Blocked)
            {
                // TODO: send notification: login attempt made
            }

            if (!isPasswordValid)
                throw new UnauthorizedException("Invalid credentials.");

            if (lastLoginIpAddress is not null && lastLoginIpAddress.Equals(user.Login.LastLoginIpAddress))
            {
                // TODO: send notification: login attempt made from new IP address
            }

            return user;
        }

        private void CheckUserStatus(UserStatus status)
        {
            if (status.Value == UserStatuses.Blocked
             && status.Value == UserStatuses.Disabled)
            {
                throw new ForbiddenException($"User status is '{status.Value}'.");
            }
        }

        private async Task ProcessLoginAttempt(User user, bool isPasswordValid)
        {
            if (isPasswordValid)
            {
                user.Login.WrongLoginAttemptsCounter = 0;
                user.Login.LastLoginDate = DateTime.UtcNow;

                string? userIpAddress = _httpContextAccessor?.HttpContext?.GetUserIpAddress();
                if (!string.IsNullOrWhiteSpace(userIpAddress))
                    user.Login.LastLoginIpAddress = userIpAddress;
            }
            else
            {
                user.Login.WrongLoginAttemptsCounter++;

                if (user.Login.WrongLoginAttemptsCounter >= _appSettingsOptions.Security.DefaultMaxWrongLoginAttemptsBeforeBlock)
                    user.Block(UserStatusReasons.MaxWrongLoginAttemptsReached);
            }

            await _identityDbContext.SaveChangesAsync();
        }
    }
}