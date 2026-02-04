using Database.IdentityDb.DefaultSchema;
using Infrastructure;

namespace Business
{
    public static class ClientValidator
    {
        public static bool IsAuthAllowed(this ClientStatus status)
        {
            return status.Value != ClientStatuses.Blocked
                && status.Value != ClientStatuses.Disabled;
        }
    }
}