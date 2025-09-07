using Database.AuthDb.DefaultSchema;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Business
{
    public interface IJwtProvider
    {
        string CreateAccessToken(Client client);
        string CreateAccessToken(User user);
        string CreateRefreshToken(User user);

        Task<TokenValidationResult> ValidateAccessTokenAsync(string authorization);

        JwtSecurityToken? DecodeRefreshToken(string authorization);
    }
}