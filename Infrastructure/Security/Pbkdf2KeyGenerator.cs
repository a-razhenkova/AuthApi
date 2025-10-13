using System.Security.Cryptography;
using System.Text;

namespace Business
{
    public static class Pbkdf2KeyGenerator
    {
        public static (string Pbkdf2Key, string Secret) CreateNewPbkdf2Key(string value, int interactions = 1000, int hashLength = 128, int saltLength = 16)
            => CreatePbkdf2Key(value, secret: null, interactions, hashLength, saltLength);

        public static (string Pbkdf2Key, string Secret) RecreatePbkdf2Key(string value, string secret, int interactions = 1000, int hashLength = 128)
            => CreatePbkdf2Key(value, secret, interactions, hashLength);

        public static bool IsPbkdf2KeyValid(string pbkdf2Key, string originalValue, string secret, int interactions = 1000, int hashLength = 128)
        {
            string recreatePbkdf2Key = RecreatePbkdf2Key(originalValue, secret, interactions, hashLength).Pbkdf2Key;
            return pbkdf2Key.Equals(recreatePbkdf2Key);
        }

        private static (string Pbkdf2Key, string Secret) CreatePbkdf2Key(string value, string? secret = null, int interactions = 1000, int hashLength = 128, int saltLength = 16)
        {
            if (string.IsNullOrWhiteSpace(value) || interactions <= 0 || hashLength <= 0)
                throw new InvalidOperationException();

            if (string.IsNullOrWhiteSpace(secret) && saltLength < 0)
                throw new InvalidOperationException();

            Span<byte> salt = string.IsNullOrWhiteSpace(secret) ? SecretGenerator.CreateNewSecret(saltLength) : Convert.FromBase64String(secret);
            ReadOnlySpan<byte> pbkdf2Key = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(value), salt, interactions, HashAlgorithmName.SHA256, hashLength);

            return (Convert.ToBase64String(pbkdf2Key), Convert.ToBase64String(salt));
        }
    }
}