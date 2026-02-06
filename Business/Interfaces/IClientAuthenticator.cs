using Database.IdentityDb.DefaultSchema;

namespace Business
{
    public interface IClientAuthenticator
    {
        Task<Client> AuthenticateAsync(string key);
        Task<Client> AuthenticateAsync(string key, string secret);
    }
}