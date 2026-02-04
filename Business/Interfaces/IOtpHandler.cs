namespace Business
{
    public interface IOtpHandler
    {
        Task<string> CreateAndSendOtpAsync(string username, string password);
    }
}