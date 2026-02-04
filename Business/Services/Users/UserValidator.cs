using Database.IdentityDb.DefaultSchema;
using Infrastructure;

namespace Business
{
    public static class UserValidator
    {
        public static bool IsAuthAllowed(this UserStatus status)
        {
            return status.Value != UserStatuses.Blocked
                && status.Value != UserStatuses.Disabled;
        }

        public static bool IsOtpAuthAllowed(this UserStatus status)
        {
            return status.IsAuthAllowed()
                && status.Value != UserStatuses.Restricted;
        }
    }
}