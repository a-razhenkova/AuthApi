using Database.IdentityDb;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace WebApi.Tests
{
    public static class DatabaseMock
    {
        public static IdentityDbContext GetAuthDbContext()
        {
            var options = new DbContextOptionsBuilder<IdentityDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .LogTo(src => Debug.WriteLine(src))
                .EnableSensitiveDataLogging()
                .Options;

            return new IdentityDbContext(options);
        }
    }
}