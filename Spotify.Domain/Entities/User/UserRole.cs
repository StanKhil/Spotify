using Microsoft.AspNetCore.Identity;

namespace Spotify.Domain.Entities.User
{
    public class UserRole : IdentityRole<Guid>
    {
        public string Description { get; set; } = null!;

        public bool CanCreate { get; set; }
        public bool CanRead { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }
}
