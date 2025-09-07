namespace Business
{
    public static class UserSecretGenerator
    {
        private const int UserSecretLength = 16;

        public static string CreateNewBase64Secret()
            => SecretGenerator.CreateNewBase64Secret(UserSecretLength);
    }
}