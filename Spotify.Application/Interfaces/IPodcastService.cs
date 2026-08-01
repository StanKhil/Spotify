using Spotify.Application.DTOs.Podcast;

namespace Spotify.Application.Interfaces;

public interface IPodcastService
{
    Task<IReadOnlyCollection<PodcastResponse>> GetPodcastsAsync(CancellationToken cancellationToken = default);
    Task<PodcastResponse?> GetPodcastByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CreatePodcastResult> CreatePodcastAsync(CreatePodcastRequest request, CancellationToken cancellationToken = default);
    Task<UpdatePodcastResult> EditPodcastAsync(Guid id, UpdatePodcastRequest request, CancellationToken cancellationToken = default);
    Task<DeletePodcastResult> DeletePodcastAsync(Guid id, CancellationToken cancellationToken = default);
}