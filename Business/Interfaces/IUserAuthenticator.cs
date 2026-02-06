using Database.IdentityDb.DefaultSchema;

namespace Business
{
    public interface IUserAuthenticator
    {
        Task<User> AuthenticateAsync(string externalId);
        Task<User> AuthenticateAsync(string username, string password);
    }
}