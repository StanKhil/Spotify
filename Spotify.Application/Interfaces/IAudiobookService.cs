using Spotify.Application.DTOs.Audiobook;

namespace Spotify.Application.Interfaces;

public interface IAudiobookService
{
    Task<IReadOnlyCollection<AudiobookResponse>> GetAudiobooksAsync(CancellationToken cancellationToken = default);
    Task<AudiobookResponse?> GetAudiobookByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CreateAudiobookResult> CreateAudiobookAsync(CreateAudiobookRequest request, CancellationToken cancellationToken = default);
    Task<UpdateAudiobookResult> EditAudiobookAsync(Guid id, UpdateAudiobookRequest request, CancellationToken cancellationToken = default);
    Task<DeleteAudiobookResult> DeleteAudiobookAsync(Guid id, CancellationToken cancellationToken = default);
}