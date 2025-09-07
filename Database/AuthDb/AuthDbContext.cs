using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Database.AuthDb
{
    [Database(ConnectionStringNames.AuthDb)]
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) 
            : base(options)
        {

        }

        #region DefaultSchema

        public DbSet<DefaultSchema.Login> Login { get; set; }

        public DbSet<DefaultSchema.Client> Client { get; set; }
        public DbSet<DefaultSchema.ClientStatus> ClientStatus { get; set; }
        public DbSet<DefaultSchema.ClientRight> ClientRight { get; set; }
        public DbSet<DefaultSchema.ClientSubscription> ClientSubscription { get; set; }

        public DbSet<DefaultSchema.User> User { get; set; }
        public DbSet<DefaultSchema.UserPassword> UserPassword { get; set; }
        public DbSet<DefaultSchema.UserStatus> UserStatus { get; set; }

        #endregion
    }
}