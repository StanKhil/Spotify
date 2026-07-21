using System;

namespace Spotify.Domain.Entities.Content
{
    public class AudioItem
    {
        public Guid Id { get; set; }
        public string AudioList { get; set; } = null!;
    }
}