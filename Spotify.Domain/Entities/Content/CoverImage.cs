using System;
using System.Collections.Generic;

namespace Spotify.Domain.Entities.Content
{
    public class CoverImage
    {
        public Guid Id { get; set; }
        public string ImageList { get; set; } = null!;

        public ICollection<Album> Albums { get; set; } = new List<Album>();
    }
}