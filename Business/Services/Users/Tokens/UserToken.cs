using Database.IdentityDb.DefaultSchema;
using Infrastructure.Configuration.AppSettings;
using System.Security.Claims;

namespace Business
{
    public abstract class UserToken : IToken
    {
        protected readonly User _user;

        protected UserToken(User user, TokenOptions options)
        {
            _user = user;
            TokenOptions = options;
        }

        public TokenOptions TokenOptions { get; init; }
        
        public abstract List<Claim> CreateClaims();
    }
}