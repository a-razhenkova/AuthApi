using System.Security.Cryptography;

namespace Business
{
    public static class SecretGenerator
    {
        public static string CreateNewBase64Secret(int size)
        {
            ReadOnlySpan<byte> secret = CreateNewSecret(size);
            return Convert.ToBase64String(secret);
        }

        public static Span<byte> CreateNewSecret(int size)
        {
            Span<byte> secret = new byte[size];

            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetNonZeroBytes(secret);

            return secret;
        }
    }
}