using Spotify.Domain.Entities.User;

namespace Spotify.Domain.Entities.Location
{
    public class Country
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<City> Cities { get; set; } = [];
        public ICollection<UserProfile> Users { get; set; } = [];
    }
}
