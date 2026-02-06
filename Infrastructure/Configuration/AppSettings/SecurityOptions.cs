using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Configuration.AppSettings
{
    public record SecurityOptions
    {
        [Required]
        public string ClientId { get; init; } = string.Empty;

        [Required]
        public string ClientSecret { get; init; } = string.Empty;

        public RateLimiterOptions RateLimiter { get; init; } = new();

        [Range(minimum: 1, maximum: int.MaxValue)]
        public int DefaultMaxWrongLoginAttemptsBeforeBlock { get; init; } = 3;

        [Required]
        public string PasswordValidationRegex { get; init; } = string.Empty;

        [Required]
        public string TokenIssuer { get; init; } = string.Empty;

        [Required]
        public string TokenAudience { get; init; } = string.Empty;

        public SecurityTokenOptions AccessToken { get; init; } = new();

        public SecurityTokenOptions RefreshToken { get; init; } = new();

        public MultiFactorAuthOptions MultiFactorAuth { get; init; } = new();
    }

    public record RateLimiterOptions
    {
        [Range(minimum: 1, maximum: int.MaxValue)]
        public int WindowInSeconds { get; init; } = 30;

        [Range(minimum: 1, maximum: int.MaxValue)]
        public int RequestsPerWindow { get; init; } = 15;
    }

    public record SecurityTokenOptions
    {
        [MinLength(16)]
        public string Key { get; init; } = string.Empty;

        [Range(minimum: 1, maximum: int.MaxValue)]
        public int LifetimeInSeconds { get; init; }
    }

    public record MultiFactorAuthOptions
    {
        [Range(minimum: 1, maximum: int.MaxValue)]
        public int LifetimeInSeconds { get; init; } = 30;

        [Range(minimum: 1, maximum: int.MaxValue)]
        public int DefaultMaxWrongLoginAttemptsBeforeBlock { get; init; } = 3;
    }
}