using Spotify.Application.DTOs.UserProfile;

namespace Spotify.Application.Interfaces
{
    public interface IUserProfileService
    {
        public Task<UpdateUserProfileResult> UpdateUserProfileAsync(UpdateUserProfileRequest request, CancellationToken cancellationToken);

        public Task<UserProfileStatisticsResult> GetUserProfileStatisticsAsync(Guid userId, CancellationToken cancellationToken);
    }
}
