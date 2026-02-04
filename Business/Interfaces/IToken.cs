using Infrastructure.Configuration.AppSettings;
using System.Security.Claims;

namespace Business
{
    public interface IToken
    {
        TokenOptions TokenOptions { get; init; }

        List<Claim> CreateClaims();
    }
}