using Database.AuthDb.DefaultSchema;
using Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Business
{
    public class AuthService : IAuthenticator
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IClientAuthenticator _clientAuthenticator;
        private readonly IUserAuthenticator _userAuthenticator;
        private readonly IJwtProvider _jwtProvider;
        private readonly IOtpProvider _otpProvider;

        public AuthService(IHttpContextAccessor httpContextAccessor,
                          IClientAuthenticator clientAuthenticator,
                          IUserAuthenticator userAuthenticator,
                          IJwtProvider jwtProvider,
                          IOtpProvider otpProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _clientAuthenticator = clientAuthenticator;
            _userAuthenticator = userAuthenticator;
            _jwtProvider = jwtProvider;
            _otpProvider = otpProvider;
        }

        public async Task<TokenDto> CreateAccessTokenAsync(string authorization)
        {
            if (string.IsNullOrWhiteSpace(authorization))
                throw new UnauthorizedException();

            if (!authorization.IsBasicAuth())
                throw new BadRequestException("Invalid token format.");

            Client client = await _clientAuthenticator.AuthAsync(authorization);

            return new TokenDto()
            {
                AccessToken = _jwtProvider.CreateAccessToken(client)
            };
        }

        public async Task<TokenDto> CreateAccessTokenAsync(string username, string password)
        {
            User user = await _userAuthenticator.AuthAsync(username, password);

            return new TokenDto()
            {
                AccessToken = _jwtProvider.CreateAccessToken(user),
                RefreshToken = _jwtProvider.CreateRefreshToken(user)
            };
        }

        public async Task<TokenDto> CreateAccessTokenByOtpAsync(string userExternalId, string otp)
        {
            User user = await _otpProvider.ValidateOtpAsync(userExternalId, otp);

            return new TokenDto()
            {
                AccessToken = _jwtProvider.CreateAccessToken(user),
                RefreshToken = _jwtProvider.CreateRefreshToken(user)
            };
        }

        public async Task<TokenValidationResult> ValidateAccessTokenAsync()
        {
            string authorization = _httpContextAccessor.HttpContext.GetAuthorization();
            if (string.IsNullOrWhiteSpace(authorization))
                throw new UnauthorizedException();

            return await _jwtProvider.ValidateAccessTokenAsync(authorization);
        }

        public async Task<TokenDto> RefreshAccessTokenAsync()
        {
            string authorization = _httpContextAccessor.HttpContext.GetAuthorization();
            if (string.IsNullOrWhiteSpace(authorization))
                throw new UnauthorizedException();

            if (!authorization.IsBearerAuth())
                throw new BadRequestException("Invalid token format.");

            JwtSecurityToken jwt = _jwtProvider.DecodeRefreshToken(authorization)
                ?? throw new UnauthorizedException("Invalid token.");

            User user = await _userAuthenticator.AuthAsync(jwt);

            return new TokenDto()
            {
                AccessToken = _jwtProvider.CreateAccessToken(user)
            };
        }

        public async Task<string> CreateAndSendOtpAsync(string username, string password)
        {
            User user = await _userAuthenticator.AuthAsync(username, password);

            if (!user.Status.IsOtpAuthAllowed())
                throw new ForbiddenException($"User status is '{user.Status.Value}'.");

            return await _otpProvider.CreateAndSendOtpAsync(user);
        }
    }
}