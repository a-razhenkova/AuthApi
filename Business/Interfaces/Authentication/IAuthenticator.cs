using Microsoft.IdentityModel.Tokens;

namespace Business
{
    public interface IAuthenticator
    {
        Task<TokenDto> CreateAccessTokenAsync(string authorization);

        Task<TokenDto> CreateAccessTokenAsync(string username, string password);
        Task<TokenDto> CreateAccessTokenByOtpAsync(string userExternalId, string otp);

        Task<TokenValidationResult> ValidateAccessTokenAsync();

        Task<TokenDto> RefreshAccessTokenAsync();

        Task<string> CreateAndSendOtpAsync(string username, string password);
    }
}