using Database.IdentityDb.DefaultSchema;
using Infrastructure.Configuration.AppSettings;
using System.Security.Claims;

namespace Business
{
    public abstract class UserToken : ISecurityToken
    {
        protected readonly User _user;

        protected UserToken(User user, SecurityTokenOptions options)
        {
            _user = user;
            TokenOptions = options;
        }

        public SecurityTokenOptions TokenOptions { get; init; }
        
        public abstract List<Claim> CreateClaims();
    }
}