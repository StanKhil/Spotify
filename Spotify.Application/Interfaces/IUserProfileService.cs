using Spotify.Application.DTOs.UserProfile;

namespace Spotify.Application.Interfaces
{
    public interface IUserProfileService
    {
        public Task<UpdateUserProfileResult> UpdateUserProfileAsync(UpdateUserProfileRequest request, CancellationToken cancellationToken);
    }
}
