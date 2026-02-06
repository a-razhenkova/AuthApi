using Database.IdentityDb.DefaultSchema;
using Infrastructure.Configuration.AppSettings;
using System.Security.Claims;

namespace Business
{
    public abstract class ClientToken : ISecurityToken
    {
        protected readonly Client _client;

        protected ClientToken(Client client, SecurityTokenOptions options)
        {
            _client = client;
            TokenOptions = options;
        }

        public SecurityTokenOptions TokenOptions { get; init; }
        
        public abstract List<Claim> CreateClaims();
    }
}