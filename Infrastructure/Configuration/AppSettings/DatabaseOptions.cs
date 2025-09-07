namespace Infrastructure.Configuration.AppSettings
{
    public record DatabaseOptions
    {
        public bool IsDbMigrationAllowed { get; init; } = false;

        public bool IsDbUpAllowed { get; init; } = false;
    }
}