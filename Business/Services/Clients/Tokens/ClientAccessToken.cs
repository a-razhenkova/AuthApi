using Database.IdentityDb.DefaultSchema;
using Infrastructure;
using Infrastructure.Configuration.AppSettings;
using System.Security.Claims;

namespace Business
{
    public class ClientAccessToken : ClientToken
    {
        public ClientAccessToken(Client client, TokenOptions options)
            : base(client, options)
        {

        }

        public override List<Claim> CreateClaims() => [
                new Claim(TokenClaim.ClientId.GetDescription(), _client.Key),
                new Claim(TokenClaim.IsInternalClient.GetDescription(), _client.IsInternal.ToString(), ClaimValueTypes.Boolean),
                new Claim(TokenClaim.CanNotifyParty.GetDescription(), _client.Right.CanNotifyParty.ToString(), ClaimValueTypes.Boolean),
            ];
    }
}