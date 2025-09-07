using Database.AuthDb.DefaultSchema;

namespace Business
{
    public interface IClientAuthenticator
    {
        Task<Client> AuthAsync(string authorization);
    }
}