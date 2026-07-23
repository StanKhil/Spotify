using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Domain.Entities.Content
{
    public class UserRole : IdentityRole<Guid>
    {
        public string Description { get; set; } = null!;
        public bool CanCreate { get; set; }
        public bool CanRead { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }

        public List<UserAccess> UserAccesses { get; set; } = [];
    }
}
