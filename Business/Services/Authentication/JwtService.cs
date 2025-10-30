using Database.AuthDb.DefaultSchema;
using Infrastructure;
using Infrastructure.Configuration.AppSettings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Business
{
    public class JwtService : IJwtProvider
    {
        private readonly AppSettingsOptions _appSettingsOptions;

        public JwtService(IOptionsSnapshot<AppSettingsOptions> appSettingsOptions)
        {
            _appSettingsOptions = appSettingsOptions.Value;
        }

        public string CreateAccessToken(Client client)
        {
            return CreateJwt(_appSettingsOptions.Security.AccessToken,
            [
                new Claim(TokenClaim.ClientId.GetDescription(), client.Key),
                new Claim(TokenClaim.IsInternalClient.GetDescription(), client.IsInternal.ToString(), ClaimValueTypes.Boolean),
                new Claim(TokenClaim.CanNotifyParty.GetDescription(), client.Right.CanNotifyParty.ToString(), ClaimValueTypes.Boolean),
            ]);
        }

        public string CreateAccessToken(User user)
        {
            return CreateJwt(_appSettingsOptions.Security.AccessToken,
            [
                new Claim(TokenClaim.UserExternalId.GetDescription(), user.ExternalId),
                new Claim(TokenClaim.Username.GetDescription(), user.Username),
                new Claim(TokenClaim.UserRole.GetDescription(), user.Role.ToString()),
                new Claim(TokenClaim.UserStatus.GetDescription(), user.Status.Value.ToString())
            ]);
        }

        public string CreateRefreshToken(User user)
        {
            return CreateJwt(_appSettingsOptions.Security.RefreshToken,
            [
                new Claim(TokenClaim.UserExternalId.GetDescription(), user.ExternalId)
            ]);
        }

        public async Task<TokenValidationResult> ValidateAccessTokenAsync(string authorization)
        {
            TokenValidationParameters tokenValidationParams = GetAccessValidationParameters(_appSettingsOptions.Security);
            return await BearerAuth.ValidateJwtAsync(authorization, tokenValidationParams);
        }

        public JwtSecurityToken? DecodeRefreshToken(string authorization)
        {
            JwtSecurityToken? token = null;

            TokenValidationParameters tokenValidationParams = GetValidationParameters(_appSettingsOptions.Security);
            tokenValidationParams.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettingsOptions.Security.RefreshToken.Key));

            try
            {
                token = BearerAuth.ValidateAndDecodeJwt(authorization, tokenValidationParams);
            }
            catch (Exception)
            {
                // continue
            }

            return token;
        }

        public static TokenValidationParameters GetAccessValidationParameters(SecurityOptions securityOptions)
        {
            TokenValidationParameters tokenValidationParams = GetValidationParameters(securityOptions);
            tokenValidationParams.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityOptions.AccessToken.Key));
            return tokenValidationParams;
        }

        private static TokenValidationParameters GetValidationParameters(SecurityOptions securityOptions)
        {
            return new TokenValidationParameters()
            {
                RequireSignedTokens = true,
                SaveSigninToken = true,
                LogValidationExceptions = false,
                ValidateLifetime = true,
                ValidateIssuer = true,
                ValidIssuer = securityOptions.TokenIssuer,
                ValidateAudience = true,
                ValidAudience = securityOptions.TokenAudience,
                ValidateIssuerSigningKey = true
            };
        }

        private string CreateJwt(TokenOptions tokenOptions, List<Claim>? claims = null)
        {
            DateTime currentTimestamp = DateTime.UtcNow;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.Key));
            var header = new JwtHeader(new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            var payload = new JwtPayload(issuer: _appSettingsOptions.Security.TokenIssuer,
                audience: _appSettingsOptions.Security.TokenAudience,
                claims: claims,
                notBefore: currentTimestamp,
                expires: currentTimestamp.AddSeconds(tokenOptions.LifetimeInSeconds),
                issuedAt: currentTimestamp);

            var token = new JwtSecurityToken(header, payload);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}