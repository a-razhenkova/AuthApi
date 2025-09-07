using Database.AuthDb.DefaultSchema;

namespace Business
{
    public interface IOtpProvider
    {
        Task<string> CreateAndSendOtpAsync(User user);

        Task<User> ValidateOtpAsync(string userExternalId, string otp);
    }
}