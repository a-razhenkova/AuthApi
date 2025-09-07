﻿using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Configuration.AppSettings
{
    public record LogOptions
    {
        public LogFileOptions File { get; init; } = new();
    }

    public record LogFileOptions
    {
        [Required]
        public string Path { get; init; } = string.Empty;

        [Range(minimum: 1, maximum: int.MaxValue)]
        public int MaxMbSize { get; init; } = 128;
    }
}