using Database.AuthDb.DefaultSchema;
using Infrastructure;

namespace Business
{
    public interface IBasicAuthenticator
    {
        Task<Client> AuthenticateAsync(Authorization authorization);
        Task<Client> AuthenticateAsync(string clientKey, string clientSecret);
    }
}