using Spotify.Application.DTOs.Media;

namespace Spotify.Application.Interfaces;

public interface IMediaService
{
    Task<MediaUploadResult> UploadAudioAsync(Stream content, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<MediaUploadResult> UploadImageAsync(Stream content, string fileName, string contentType, CancellationToken cancellationToken = default);
}