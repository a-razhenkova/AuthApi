using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Configuration.AppSettings
{
    public record BusinessOptions
    {
        public PaginatedReportOptions PaginatedReport { get; init; } = new();
    }


}