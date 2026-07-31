using Spotify.Application.DTOs.Playlist;

namespace Spotify.Application.Interfaces;

public interface IPlaylistService
{
    Task<IReadOnlyCollection<PlaylistResponse>> GetPlaylistsAsync(CancellationToken cancellationToken = default);
    Task<PlaylistResponse?> GetPlaylistByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CreatePlaylistResult> CreatePlaylistAsync(CreatePlaylistRequest request, CancellationToken cancellationToken = default);
    Task<UpdatePlaylistResult> EditPlaylistAsync(Guid id, UpdatePlaylistRequest request, CancellationToken cancellationToken = default);
    Task<DeletePlaylistResult> DeletePlaylistAsync(Guid id, CancellationToken cancellationToken = default);
}