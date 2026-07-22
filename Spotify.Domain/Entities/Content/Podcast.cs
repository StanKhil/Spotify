using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Domain.Entities.Content
{
    public class Podcast
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public ICollection<Episode> Episodes { get; set; } = new List<Episode>();
    }
}
