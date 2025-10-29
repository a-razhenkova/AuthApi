using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Configuration.AppSettings
{
    public record AppSettingsOptions
    {
        public LogOptions Log { get; init; } = new();

        public SecurityOptions Security { get; init; } = new();

        public DatabaseOptions Database { get; init; } = new();

        [Required]
        public string ClientSubscriptionContractDirectory { get; set; } = string.Empty;

        public PaginatedReportOptions PaginatedReport { get; init; } = new();

        public ExternalServicesOptions ExternalServices { get; init; } = new();
    }

    public record PaginatedReportOptions
    {
        [Range(minimum: 1, maximum: 1000)]
        public int DefaultItemsPerPage { get; init; } = 20;

        [Range(minimum: 1, maximum: 1000)]
        public int DefaultMaxAllowedItemsPerPage { get; init; } = 1000;
    }
}