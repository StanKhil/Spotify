using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Domain.Entities.User
{
    public class License
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = null!;
        public string UserEmail { get; set; } = null!;
        public string ActivationKey { get; set; } = null!;
    }
}
