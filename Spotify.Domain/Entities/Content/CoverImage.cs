using System;

namespace Spotify.Domain.Entities.Content
{
    public class CoverImage
    {
        public Guid Id { get; set; }
        public string ImageList { get; set; } = null!;
    }
}