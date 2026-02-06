using Database.IdentityDb.DefaultSchema;
using Infrastructure;
using Infrastructure.Configuration.AppSettings;
using System.Security.Claims;

namespace Business
{
    public class UserAccessToken : UserToken
    {
        public UserAccessToken(User user, SecurityTokenOptions options)
            : base(user, options)
        {

        }

        public override List<Claim> CreateClaims() => [
                new Claim(TokenClaim.UserExternalId.GetDescription(), _user.ExternalId),
                new Claim(TokenClaim.Username.GetDescription(), _user.Username),
                new Claim(TokenClaim.UserRole.GetDescription(), _user.Role.ToString()),
                new Claim(TokenClaim.UserStatus.GetDescription(), _user.Status.Value.ToString())
            ];
    }
}