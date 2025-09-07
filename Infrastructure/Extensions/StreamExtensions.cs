namespace Infrastructure
{
    public static class StreamExtensions
    {
        public static async Task<string> ReadToEndAsync(this Stream value)
        {
            value.Position = 0;

            var streamReader = new StreamReader(value);
            string valueString = await streamReader.ReadToEndAsync();

            value.Position = 0;
            return valueString;
        }
    }
}