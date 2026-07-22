using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Domain.Entities.Content
{
    public class UserAccess
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid RoleId { get; set; }

        public string Login { get; set; } = null!;

        public string Salt { get; set; } = null!;

        public string DerivedKey { get; set; } = null!;

        public UserData User { get; set; } = null!;

        public UserRole Role { get; set; } = null!;
    }
}
