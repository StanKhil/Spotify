namespace Spotify.Application.DTOs.AuthorUpload;

public sealed record AuthorAlbumUpdateRequest(
    Guid AlbumId,
    string Name,
    string? Description,
    string? GenreId,
    UploadedFile? Cover);