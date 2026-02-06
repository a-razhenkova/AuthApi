using Infrastructure.Configuration.AppSettings;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Business
{
    public class SecurityTokenHandler
    {
        private readonly ISecurityToken _token;
        private readonly SecurityOptions _options;

        public SecurityTokenHandler(ISecurityToken token, SecurityOptions options)
        {
            _token = token;
            _options = options;
        }

        public string Create()
        {
            DateTime currentTimestamp = DateTime.UtcNow;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_token.TokenOptions.Key));
            var header = new JwtHeader(new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            var payload = new JwtPayload(issuer: _options.TokenIssuer,
                audience: _options.TokenAudience,
                claims: _token.CreateClaims(),
                notBefore: currentTimestamp,
                expires: currentTimestamp.AddSeconds(_token.TokenOptions.LifetimeInSeconds),
                issuedAt: currentTimestamp);

            var token = new JwtSecurityToken(header, payload);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}