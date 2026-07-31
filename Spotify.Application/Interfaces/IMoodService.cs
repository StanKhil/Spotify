using Spotify.Application.DTOs.Mood;

namespace Spotify.Application.Interfaces;

public interface IMoodService
{
    Task<IReadOnlyCollection<MoodResponse>> GetMoodsAsync(
        CancellationToken cancellationToken = default);

    Task<CreateMoodResult> CreateMoodAsync(
        CreateMoodRequest request,
        CancellationToken cancellationToken = default);

    Task<UpdateMoodResult> EditMoodAsync(
        Guid id,
        UpdateMoodRequest request,
        CancellationToken cancellationToken = default);

    Task<DeleteMoodResult> DeleteMoodAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}