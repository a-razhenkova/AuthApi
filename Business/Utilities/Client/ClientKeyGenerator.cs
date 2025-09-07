namespace Business
{
    public static class ClientKeyGenerator
    {
        public static string CreateNewKey()
            => Guid.NewGuid().ToString();
    }
}