using Spotify.Application.DTOs.Episode;

namespace Spotify.Application.Interfaces;

public interface IEpisodeService
{
    Task<IReadOnlyCollection<EpisodeResponse>> GetEpisodesAsync(Guid podcastId, CancellationToken cancellationToken = default);
    Task<EpisodeResponse?> GetEpisodeByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CreateEpisodeResult> CreateEpisodeAsync(CreateEpisodeRequest request, CancellationToken cancellationToken = default);
    Task<UpdateEpisodeResult> EditEpisodeAsync(Guid id, UpdateEpisodeRequest request, CancellationToken cancellationToken = default);
    Task<DeleteEpisodeResult> DeleteEpisodeAsync(Guid id, CancellationToken cancellationToken = default);
}