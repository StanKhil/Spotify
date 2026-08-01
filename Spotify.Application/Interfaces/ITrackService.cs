using Spotify.Application.DTOs.Track;

namespace Spotify.Application.Interfaces;

public interface ITrackService
{
    Task<IReadOnlyCollection<TrackResponse>> GetTracksAsync(CancellationToken cancellationToken = default);
    Task<TrackResponse?> GetTrackByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CreateTrackResult> CreateTrackAsync(CreateTrackRequest request, CancellationToken cancellationToken = default);
    Task<UpdateTrackResult> EditTrackAsync(Guid id, UpdateTrackRequest request, CancellationToken cancellationToken = default);
    Task<DeleteTrackResult> DeleteTrackAsync(Guid id, CancellationToken cancellationToken = default);
    Task<BatchDeleteTracksResult> BatchDeleteTracksAsync(BatchDeleteTracksRequest request, CancellationToken cancellationToken = default);
}