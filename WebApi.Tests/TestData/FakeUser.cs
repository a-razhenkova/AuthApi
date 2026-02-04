using Database.IdentityDb;
using Database.IdentityDb.DefaultSchema;
using Infrastructure;

namespace WebApi.Tests
{
    public static class FakeUser
    {
        public const string Username = "ivan.ivanov";
        public const string Password = "common";
        public const string Secret = "testsecret";

        public static User Create()
        {
            return new()
            {
                ExternalId = Guid.NewGuid().ToString(),
                Username = Username,
                Role = UserRoles.Administrator,
                OtpSecret = "ORSXG5DTMVRXEZLU", /* DefaultSecret */
                Status = new UserStatus(),
                Password = new UserPassword()
                {
                    Value = "ZVBgBh5Juq3MjgLYSxfkrTLMUnYkHOZ719ysyrkGaif8D5DBm2cels7IBfcAK4BFSFJuwiDTNUrjJeQ7yVJU7HnNPhJhAosGFcd3uTii8VmJSlZ+Z7Q4DwBUQXYvr+2O5PEfPfZJdFfIuhQu7XpUrt5mTa8SO/SnJrH8v9tlVpc=", /* DefaultPassword */
                    Secret = "LudClePZUDxe5KOXLfYDBQ==",
                }
            };
        }

        public static async Task<User> CreateAndSave(IdentityDbContext authDbContext)
        {
            User user = Create();

            await authDbContext.AddAsync(user);
            await authDbContext.SaveChangesAsync();

            return user;
        }
    }
}