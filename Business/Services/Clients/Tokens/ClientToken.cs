using Database.AuthDb.DefaultSchema;
using Infrastructure.Configuration.AppSettings;
using System.Security.Claims;

namespace Business
{
    public abstract class ClientToken : IToken
    {
        protected readonly Client _client;

        protected ClientToken(Client client, TokenOptions options)
        {
            _client = client;
            TokenOptions = options;
        }

        public TokenOptions TokenOptions { get; init; }
        
        public abstract List<Claim> CreateClaims();
    }
}