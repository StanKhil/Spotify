using Spotify.Application.DTOs.Auth;
using Spotify.Domain.Entities.User;


namespace Spotify.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        public abstract AuthenticationResponse Create(ApplicationUser user, IEnumerable<string> roles);
    }
}
