using System;

namespace Spotify.Domain.Entities.Content
{
    public class Mood
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public Guid? MoodImageId { get; set; }
        public ImageItem? MoodImage { get; set; }
    }
}