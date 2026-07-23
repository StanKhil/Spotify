using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;
using System.Text;

namespace Spotify.Domain.Entities.Content
{
    public class UserData
    {
        public Guid Id { get; set; }
        [Required]
        public string Email { get; set; } = null!;
        public Guid CountryId { get; set; }
        public Guid CityId { get; set; }
        public DateTime Birthdate { get; set; }
        public bool IsAdult { get; set; }
        public DateTime RegisteredAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(CountryId))]
        public Country Country { get; set; } = null!;

        [ForeignKey(nameof(CityId))]
        public City City { get; set; } = null!;
        public UserAccess UserAccess { get; set; } = null!;
    }
}
