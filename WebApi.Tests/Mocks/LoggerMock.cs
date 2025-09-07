using Microsoft.Extensions.Logging;
using Moq;

namespace WebApi.Tests
{
    public static class LoggerMock
    {
        public static ILogger<TType> GetLogger<TType>()
        {
            return new Mock<ILogger<TType>>().Object;
        }
    }
}