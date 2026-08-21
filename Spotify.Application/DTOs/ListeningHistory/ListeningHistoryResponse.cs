namespace Spotify.Application.DTOs.ListeningHistory
{
    public sealed record ListeningHistoryResponse(
        Guid Id,
        Guid TrackId,
        string TrackName,
        int DurationSeconds,
        int ListenedSeconds,
        bool IsCompleted,
        DateTime PlayedAt);
}
