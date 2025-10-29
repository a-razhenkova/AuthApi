using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Configuration.AppSettings
{
    public record ExternalServicesOptions
    {
        public NotifyApiOptions NotifyApi { get; init; } = new();
    }

    public record NotifyApiOptions
    {
        [Required]
        public string Url { get; init; } = string.Empty;
    }
}