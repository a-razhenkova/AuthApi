using Database.IdentityDb.DefaultSchema;

namespace Business
{
    public class OtpService : IOtpHandler
    {
        private readonly IBearerAuthenticator _bearerAuthenticator;
        private readonly IOtpAuthenticator _otpAuthenticator;

        public OtpService(IBearerAuthenticator bearerAuthenticator,
                         IOtpAuthenticator otpAuthenticator)
        {
            _bearerAuthenticator = bearerAuthenticator;
            _otpAuthenticator = otpAuthenticator;
        }

        public async Task<string> CreateAndSendOtpAsync(string username, string password)
        {
            User user = await _bearerAuthenticator.AuthAsync(username, password);
            return await _otpAuthenticator.CreateAndSendOtpAsync(user);
        }
    }
}
