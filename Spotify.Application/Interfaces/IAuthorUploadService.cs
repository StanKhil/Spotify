using Spotify.Application.DTOs.AuthorUpload;

namespace Spotify.Application.Interfaces;

public interface IAuthorUploadService
{
    Task<GetMyAuthorAlbumsResult> GetMyAlbumsAsync(CancellationToken cancellationToken = default);

    Task<GetMyAuthorTracksResult> GetMyTracksAsync(CancellationToken cancellationToken = default);

    Task<GetMyAuthorTracksResult> GetAlbumTracksAsync(Guid albumId, CancellationToken cancellationToken = default);

    Task<CreateAuthorAlbumResult> CreateAlbumAsync(AuthorAlbumUploadRequest request, CancellationToken cancellationToken = default);

    Task<UpdateAuthorAlbumResult> UpdateAlbumAsync(AuthorAlbumUpdateRequest request, CancellationToken cancellationToken = default);

    Task<DeleteAuthorAlbumResult> DeleteAlbumAsync(Guid albumId, CancellationToken cancellationToken = default);

    Task<PublishAuthorAlbumResult> PublishAlbumAsync(Guid albumId, CancellationToken cancellationToken = default);

    Task<AddAuthorTrackResult> AddTrackAsync(AuthorTrackUploadRequest request, CancellationToken cancellationToken = default);

    Task<UpdateAuthorTrackResult> UpdateTrackAsync(AuthorTrackUpdateRequest request, CancellationToken cancellationToken = default);

    Task<DeleteAuthorTrackResult> DeleteTrackAsync(Guid trackId, CancellationToken cancellationToken = default);
}