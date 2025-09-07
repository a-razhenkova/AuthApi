using Infrastructure;
using System.Text;

namespace Business
{
    public static class BasicAuth
    {
        public const string Schema = "Basic";

        public static bool IsBasicAuth(this string authorization)
            => authorization.BeginsWith(Schema);

        public static (string Key, string Secret) Decode(string authorization)
        {
            if (string.IsNullOrWhiteSpace(authorization))
                throw new InvalidOperationException();

            string token = authorization.TrimStart($"{Schema} ");

            ReadOnlySpan<byte> encodedCredentials = Convert.FromBase64String(token);
            string decodedCredentials = Encoding.UTF8.GetString(encodedCredentials);

            string[] credentials = decodedCredentials.Split(":");
            return (credentials[0], credentials[1]);
        }
    }
}