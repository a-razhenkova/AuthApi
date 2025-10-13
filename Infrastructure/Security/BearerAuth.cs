using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Infrastructure
{
    public static class BearerAuth
    {
        public const string Schema = "Bearer";

        public static bool IsBearerAuth(this string authorization)
            => authorization.BeginsWith(Schema);

        public static string GetToken(string authorization)
            => authorization.TrimStart($"{Schema} ");

        public static async Task<TokenValidationResult> ValidateJwtAsync(string authorization, TokenValidationParameters validationParams)
        {
            string token = authorization.TrimStart($"{Schema} ");
            return await new JwtSecurityTokenHandler().ValidateTokenAsync(token, validationParams);
        }

        public static JwtSecurityToken ValidateAndDecodeJwt(string authorization, TokenValidationParameters validationParams)
        {
            string token = authorization.TrimStart($"{Schema} ");

            new JwtSecurityTokenHandler().ValidateToken(token, validationParams, out SecurityToken jwt);
            return (JwtSecurityToken)jwt;
        }
    }
}