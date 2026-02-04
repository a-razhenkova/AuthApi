using Infrastructure.Configuration.AppSettings;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Business
{
    public class SecurityToken
    {
        protected readonly SecurityOptions _options;

        public SecurityToken(SecurityOptions options, string key)
        {
            _options = options;

            ValidationParams = new()
            {
                RequireSignedTokens = true,
                SaveSigninToken = true,
                LogValidationExceptions = false,
                ValidateLifetime = true,
                ValidateIssuer = true,
                ValidIssuer = options.TokenIssuer,
                ValidateAudience = true,
                ValidAudience = options.TokenAudience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            };
        }

        public TokenValidationParameters ValidationParams { get; init; }

        public async Task<TokenValidationResult> ValidateAsync(string token)
            => await new JwtSecurityTokenHandler().ValidateTokenAsync(token, ValidationParams);

        public JwtSecurityToken? Decode(string token)
        {
            JwtSecurityToken? jwt = null;

            try
            {
                new JwtSecurityTokenHandler().ValidateToken(token, ValidationParams, out Microsoft.IdentityModel.Tokens.SecurityToken validatedToken);
                jwt = (JwtSecurityToken)validatedToken;
            }
            catch (Exception)
            {
                // continue
            }

            return jwt;
        }
    }
}