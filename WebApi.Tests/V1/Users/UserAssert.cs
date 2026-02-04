using Database.IdentityDb.DefaultSchema;
using WebApi.V1;

namespace WebApi.Tests.V1
{
    public class UserAssert : Assert
    {
        public static void Equal(User expected, UserModel actual)
        {
            Equal(expected.ExternalId, actual.Id);
            Equal(expected.Username, actual.Username);
            Equal(expected.Role, actual.Role);
            Equal(expected.Status, actual.Status);
            Equal(expected.Email, actual.Email);
        }

        public static void Equal(UserStatus expected, UserStatusModel actual)
        {
            Equal(expected.Value, actual.Value);
            Equal(expected.Reason, actual.Reason);
            Equal(expected.Note, actual.Note);
        }
    }
}