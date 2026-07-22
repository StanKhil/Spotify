using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Domain.Entities.Content
{
    public class Country
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<City> Cities { get; set; } = new List<City>();
        public ICollection<UserData> Users { get; set; }
    }
}
