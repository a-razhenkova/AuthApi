using Infrastructure.Configuration.AppSettings;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Business
{
    public class TokenStrategy
    {
        private readonly IToken _token;
        private readonly SecurityOptions _options;

        public TokenStrategy(IToken token, SecurityOptions options)
        {
            _token = token;
            _options = options;
        }

        public string Create()
        {
            DateTime currentTimestamp = DateTime.UtcNow;
            List<Claim> claims = _token.CreateClaims();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_token.TokenOptions.Key));
            var header = new JwtHeader(new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            var payload = new JwtPayload(issuer: _options.TokenIssuer,
                audience: _options.TokenAudience,
                claims: claims,
                notBefore: currentTimestamp,
                expires: currentTimestamp.AddSeconds(_token.TokenOptions.LifetimeInSeconds),
                issuedAt: currentTimestamp);

            var token = new JwtSecurityToken(header, payload);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}