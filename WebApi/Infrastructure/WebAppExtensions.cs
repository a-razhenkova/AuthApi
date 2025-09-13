﻿using Database;
using Database.AuthDb;
using DbUp;
using Infrastructure;
using Infrastructure.Configuration.AppSettings;
using Microsoft.EntityFrameworkCore;
using Serilog.Context;

namespace WebApi
{
    public static class WebAppExtensions
    {
        public static async Task<WebApplication> ApplyDbPendingMigrationsAndScriptsAsync(this WebApplication app)
        {
            var dbOptions = app.Configuration.GetRequiredSection<DatabaseOptions>(nameof(AppSettingsOptions.Database));

            IServiceScope scope = app.Services.CreateScope();
            ILogger logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            AuthDbContext authDbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

            if (dbOptions.IsDbMigrationAllowed)
            {
                using (LogContext.PushProperty(LoggerContextProperty.ActionType.ToString(), LoggerContext.DbMigration))
                {
                    await authDbContext.ApplyDbPendingMigrationsAsync(logger);
                }
            }

            if (dbOptions.IsDbUpAllowed)
            {
                using (LogContext.PushProperty(LoggerContextProperty.ActionType.ToString(), LoggerContext.DbUp))
                {
                    authDbContext.ApplyDbPendingScriptsAsync(logger, app.Configuration);
                }
            }

            return app;
        }

        private static async Task ApplyDbPendingMigrationsAsync(this DbContext dbContext, ILogger logger)
        {
            try
            {
                logger.LogInformation("Checking for pending migrations...");

                IEnumerable<string> migrations = await dbContext.Database.GetPendingMigrationsAsync();
                if (migrations.Any())
                {
                    logger.LogInformation("Applying pending migrations...");

                    await dbContext.Database.MigrateAsync();

                    string dbName = dbContext.Database.GetDbConnection().Database;
                    logger.LogInformation("Applied migrations on database '{DbName}': {Migrations}", dbName, string.Join(", ", migrations));
                }

                logger.LogInformation("Database is up to date.");
            }
            catch (Exception exception)
            {
                logger.LogError(exception, exception.Message);
            }
        }

        private static void ApplyDbPendingScriptsAsync(this DbContext dbContext, ILogger logger, IConfiguration config)
        {
            try
            {
                DatabaseAttribute dbConfig = dbContext.GetType().GetRequiredCustomAttribute<DatabaseAttribute>();
                string connectionString = config.GetConnectionString(dbConfig.ConnectionStringName) ?? throw new NotImplementedException();

                var upgrader = DeployChanges.To
                    .SqlDatabase(connectionString)
                    .JournalToSqlTable(dbConfig.DefaultSchemaName, dbConfig.ScriptsHistoryTableName)
                    .WithScriptsEmbeddedInAssembly(DatabaseAssembly.GetExecutingAssembly())
                    .LogTo(logger)
                    .Build();

                logger.LogInformation("Executing pending scripts...");

                upgrader.PerformUpgrade();

                logger.LogInformation("Database is up to date.");
            }
            catch (Exception exception)
            {
                logger.LogError(exception, exception.Message);
            }
        }
    }
}