using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Domain.Entities.Content
{
    public class Podcast
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public ICollection<Episode> Episodes { get; set; } = [];
    }
}
