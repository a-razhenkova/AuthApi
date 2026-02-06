using Database.IdentityDb.DefaultSchema;
using Infrastructure;

namespace Business
{
    public static class ClientExtentions
    {
        extension(Client client)
        {
            public void Activate()
            {
                client.Status.Value = ClientStatuses.Active;
                client.Status.Reason = ClientStatusReasons.None;
                client.Status.Note = null;
            }

            public void Block(ClientStatusReasons reason)
            {
                client.Status.Value = ClientStatuses.Disabled;
                client.Status.Reason = reason;
                client.Status.Note = null;
            }

            public bool IsSecretValid(string secret)
                => client.Secret.Equals(secret);
        }
    }
}