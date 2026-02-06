using Database.IdentityDb.DefaultSchema;
using Infrastructure;
using Infrastructure.Configuration.AppSettings;
using System.Security.Claims;

namespace Business
{
    public class UserRefreshToken : UserToken
    {
        public UserRefreshToken(User user, SecurityTokenOptions options)
            : base(user, options)
        {

        }

        public override List<Claim> CreateClaims() => [
                new Claim(TokenClaim.UserExternalId.GetDescription(), _user.ExternalId)
            ];
    }
}