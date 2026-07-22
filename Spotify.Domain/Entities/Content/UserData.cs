using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Text;

namespace Spotify.Domain.Entities.Content
{
    public class UserData
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = null!;

        public Guid CountryId { get; set; }

        public Guid CityId { get; set; }

        public DateTime Birthdate { get; set; }

        public bool IsAdult { get; set; }

        public DateTime RegisteredAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public Country Country { get; set; } = null!;

        public City City { get; set; } = null!;

        public UserAccess UserAccess { get; set; } = null!;

        public ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();

        public ICollection<AuthorContent> AuthoredContent { get; set; } = new List<AuthorContent>();
    }
}
