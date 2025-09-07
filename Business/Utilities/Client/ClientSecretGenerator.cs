namespace Business
{
    public static class ClientSecretGenerator
    {
        public static string CreateNewSecret()
            => Guid.NewGuid().ToString();
    }
}