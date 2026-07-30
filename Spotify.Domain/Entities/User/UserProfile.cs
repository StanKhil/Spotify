using Spotify.Domain.Entities.Location;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;
using System.Text;

namespace Spotify.Domain.Entities.User
{
    public class UserProfile
    {
        public Guid UserId { get; set; }

        public Guid CountryId { get; set; }
        public Guid CityId { get; set; }

        public DateTime Birthdate { get; set; }
        public bool IsAdult { get; set; }
        public DateTime RegisteredAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ApplicationUser ApplicationUser { get; set; } = null!;
        public Country Country { get; set; } = null!;
        public City City { get; set; } = null!;
    }
}
