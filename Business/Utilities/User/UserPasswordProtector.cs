using Database.AuthDb.DefaultSchema;

namespace Business
{
    public static class UserPasswordProtector
    {
        public const int Interactions = 100000; // 100,000
        public const int HashLength = 128;
        public const int SaltLength = 16;

        public static UserPassword CreateNewSecurePassword(string password)
        {
            (string hash, string salt) = Pbkdf2KeyGenerator.CreateNewPbkdf2Key(password, Interactions, HashLength, SaltLength);

            return new UserPassword()
            {
                Value = hash,
                Secret = salt,
                LastChangedTimestamp = DateTime.UtcNow
            };
        }

        public static bool IsPasswordValid(string password, string originalPassword, string salt)
            => Pbkdf2KeyGenerator.IsPbkdf2KeyValid(password, originalPassword, salt, Interactions, HashLength);
    }
}