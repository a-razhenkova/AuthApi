using Database.AuthDb;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace WebApi.Tests
{
    public static class DatabaseMock
    {
        public static AuthDbContext GetAuthDbContext()
        {
            var options = new DbContextOptionsBuilder<AuthDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .LogTo(src => Debug.WriteLine(src))
                .EnableSensitiveDataLogging()
                .Options;

            return new AuthDbContext(options);
        }
    }
}