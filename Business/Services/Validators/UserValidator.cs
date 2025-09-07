using Database.AuthDb.DefaultSchema;
using Infrastructure;

namespace Business
{
    public static class UserValidator
    {
        public static bool IsBasicAuthAllowed(this UserStatus status)
        {
            return status.Value == UserStatuses.Blocked
                || status.Value == UserStatuses.Disabled;
        }

        public static bool IsOtpAuthAllowed(this UserStatus status)
        {
            return status.IsBasicAuthAllowed()
                || status.Value == UserStatuses.Restricted;
        }
    }
}