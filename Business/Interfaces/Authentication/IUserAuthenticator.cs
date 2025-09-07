using Database.AuthDb.DefaultSchema;
using System.IdentityModel.Tokens.Jwt;

namespace Business
{
    public interface IUserAuthenticator
    {
        Task<User> AuthAsync(string username, string password);

        Task<User> AuthAsync(JwtSecurityToken jwt);
    }
}