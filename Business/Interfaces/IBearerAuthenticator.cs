using Database.IdentityDb.DefaultSchema;
using System.IdentityModel.Tokens.Jwt;

namespace Business
{
    public interface IBearerAuthenticator
    {
        Task<User> AuthAsync(string username, string password);
        Task<User> AuthAsync(JwtSecurityToken jwt);
    }
}