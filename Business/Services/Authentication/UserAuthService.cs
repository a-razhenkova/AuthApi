using Database.AuthDb;
using Database.AuthDb.DefaultSchema;
using Infrastructure;
using Infrastructure.Configuration.AppSettings;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;

namespace Business
{
    public class UserAuthService : IUserAuthenticator
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppSettingsOptions _appSettingsOptions;
        private readonly AuthDbContext _authDbContext;

        public UserAuthService(IHttpContextAccessor httpContextAccessor,
                              IOptionsSnapshot<AppSettingsOptions> appSettingsOptions,
                              AuthDbContext authDbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _appSettingsOptions = appSettingsOptions.Value;
            _authDbContext = authDbContext;
        }

        public async Task<User> AuthAsync(string username, string password)
        {
            User user = await _authDbContext.User
                .Where(u => u.Username == username)
                .Include(u => u.Status)
                .Include(u => u.Password)
                .Include(u => u.Login)
                .SingleOrDefaultAsync() ?? throw new UnauthorizedException("Invalid credentials.");

            if (user.Status.IsAuthAllowed())
                throw new ForbiddenException($"User status is {user.Status.Value}.");

            bool isPasswordValid = UserPasswordProtector.IsPasswordValid(user.Password.Value, password, user.Password.Secret);

            await ProcessLoginAttemptAsync(user, isPasswordValid);

            if (!isPasswordValid)
                throw new UnauthorizedException("Invalid credentials.");

            return user;
        }

        public async Task<User> AuthAsync(JwtSecurityToken jwt)
        {
            string userExternalId = jwt.Claims
                .Where(c => c.Type == TokenClaim.UserExternalId.GetDescription())
                .Select(c => c.Value)
                .SingleOrDefault() ?? throw new UnauthorizedException("Invalid token.");

            User user = await _authDbContext.User
               .Where(u => u.ExternalId == userExternalId)
               .Include(u => u.Status)
               .Include(u => u.Login)
               .SingleOrDefaultAsync() ?? throw new UnauthorizedException();

            if (user.Status.IsAuthAllowed())
                throw new ForbiddenException($"User status is {user.Status.Value}.");

            return user;
        }

        private async Task ProcessLoginAttemptAsync(User user, bool isLoginSuccessful)
        {
            bool isUserBlocked = false;
            bool isNewIpAddress = false;

            user.Login ??= new Login();

            if (isLoginSuccessful)
            {
                user.Login.WrongLoginAttemptsCounter = 0;
                user.Login.LastLoginDate = DateTime.UtcNow;

                string? userIpAddress = _httpContextAccessor?.HttpContext?.GetUserIpAddress();
                if (!string.IsNullOrWhiteSpace(userIpAddress))
                {
                    isNewIpAddress = user.Login.LastLoginIpAddress is null || !user.Login.LastLoginIpAddress.Equals(userIpAddress);
                    user.Login.LastLoginIpAddress = userIpAddress;
                }
            }
            else
            {
                user.Login.WrongLoginAttemptsCounter++;

                if (user.Login.WrongLoginAttemptsCounter >= _appSettingsOptions.Security.DefaultMaxWrongLoginAttemptsBeforeBlock)
                {
                    isUserBlocked = true;
                    user.Status.Value = UserStatuses.Blocked;
                    user.Status.Reason = UserStatusReasons.MaxWrongLoginAttemptsReached;
                }
            }

            await _authDbContext.SaveChangesAsync();

            if (isUserBlocked)
            {
                // TODO: send notification: login attempt made
            }
            else if (isNewIpAddress)
            {
                // TODO: send notification: login attempt made from new IP address
            }
        }
    }
}