namespace Spotify.Application.DTOs.AuthorUpload;

public sealed record UploadedFile(Stream Content, string FileName, string ContentType);