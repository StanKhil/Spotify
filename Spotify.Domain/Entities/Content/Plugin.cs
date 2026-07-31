using System;

namespace Spotify.Domain.Entities.Content
{
    // заглушка
    public class Plugin
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsEnabled { get; set; }
        public string? SettingsJson { get; set; } 
    }
}