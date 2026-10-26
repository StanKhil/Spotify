namespace Spotify.Application.DTOs.AuthorUpload;

public sealed record AuthorAlbumUploadRequest(
    string Name,
    string? Description,
    string? GenreId,
    UploadedFile Cover);