using Database.AuthDb.DefaultSchema;
using Infrastructure;
using Infrastructure.Configuration.AppSettings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Business
{
    public class TokenService : ITokenHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppSettingsOptions _appSettingsOptions;
        private readonly IBasicAuthenticator _basicAuthenticator;
        private readonly IBearerAuthenticator _bearerAuthenticator;
        private readonly IOtpAuthenticator _otpAuthenticator;

        public TokenService(IHttpContextAccessor httpContextAccessor,
                           IOptionsSnapshot<AppSettingsOptions> appSettingsOptions,
                           IBasicAuthenticator basicAuthenticator,
                           IBearerAuthenticator bearerAuthenticator,
                           IOtpAuthenticator otpAuthenticator)
        {
            _httpContextAccessor = httpContextAccessor;
            _appSettingsOptions = appSettingsOptions.Value;
            _basicAuthenticator = basicAuthenticator;
            _bearerAuthenticator = bearerAuthenticator;
            _otpAuthenticator = otpAuthenticator;
        }

        public async Task<TokenDto> CreateAccessTokenAsync(Authorization authorization)
        {
            switch (authorization.Schema)
            {
                case AuthorizationSchema.Basic:
                    {
                        Client client = await _basicAuthenticator.AuthenticateAsync(authorization);
                        return await CreateAccessTokenAsync(client);
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        public async Task<TokenDto> CreateAccessTokenAsync(Client client)
        {
            var accessToken = new AccessToken(_appSettingsOptions.Security);

            return new TokenDto()
            {
                AccessToken = accessToken.Create(client)
            };
        }

        public async Task<TokenDto> CreateAccessTokenAsync(string username, string password)
        {
            User user = await _bearerAuthenticator.AuthAsync(username, password);
            return await CreateAccessTokenAsync(user);
        }

        public async Task<TokenDto> CreateAccessTokenAsync(User user)
        {
            var accessToken = new AccessToken(_appSettingsOptions.Security);
            var refreshToken = new RefreshToken(_appSettingsOptions.Security);

            return new TokenDto()
            {
                AccessToken = accessToken.Create(user),
                RefreshToken = refreshToken.Create(user)
            };
        }

        public async Task<TokenDto> CreateAccessTokenByOtpAsync(string userExternalId, string otp)
        {
            User user = await _otpAuthenticator.ValidateOtpAsync(userExternalId, otp);
            return await CreateAccessTokenAsync(user);
        }

        public async Task<TokenDto> RefreshAccessTokenAsync()
        {
            var authorization = new Authorization(_httpContextAccessor.HttpContext.GetAuthorization());

            if (authorization.Schema != AuthorizationSchema.Bearer)
                throw new BadRequestException("Invalid token format.");

            JwtSecurityToken? jwt = new RefreshToken(_appSettingsOptions.Security).Decode(authorization.Value)
                ?? throw new UnauthorizedException("Invalid token.");

            User user = await _bearerAuthenticator.AuthAsync(jwt);

            return new TokenDto()
            {
                AccessToken = new AccessToken(_appSettingsOptions.Security).Create(user)
            };
        }

        public async Task<TokenValidationResult> ValidateAccessTokenAsync()
        {
            var authorization = new Authorization(_httpContextAccessor.HttpContext.GetAuthorization());
            return await new AccessToken(_appSettingsOptions.Security).ValidateAsync(authorization.Value);
        }
    }
}