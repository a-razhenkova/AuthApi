using Database.IdentityDb.DefaultSchema;

namespace Business
{
    public interface IOtpAuthenticator
    {
        Task<string> CreateAndSendOtpAsync(User user);

        Task<User> ValidateOtpAsync(string userExternalId, string otp);
    }
}