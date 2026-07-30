using Spotify.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Domain.Entities.Location
{
    public class City
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public Guid CountryId { get; set; }
        public Country Country { get; set; } = null!;
        public ICollection<UserProfile> Users { get; set; } = new List<UserProfile>();
    }
}