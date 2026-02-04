using Infrastructure;

namespace Business
{
    public static class ClientSecret
    {
        public static string Create()
            => Guid.NewGuid().ToString();

        public static bool IsValid(string secret)
            => GuidExtensions.IsValidUid(secret);
    }
}
