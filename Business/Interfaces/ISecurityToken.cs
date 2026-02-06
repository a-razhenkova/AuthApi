using Infrastructure.Configuration.AppSettings;
using System.Security.Claims;

namespace Business
{
    public interface ISecurityToken
    {
        SecurityTokenOptions TokenOptions { get; init; }

        List<Claim> CreateClaims();
    }
}