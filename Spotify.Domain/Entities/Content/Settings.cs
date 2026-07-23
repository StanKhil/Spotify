using Spotify.Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Domain.Entities.Content
{
    public class Settings
    {
        public Guid Id { get; set; }
        public Language Language { get; set; }
    }
}
