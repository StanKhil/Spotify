namespace Spotify.Application.DTOs.ListeningHistory
{
    public sealed record AddListeningHistoryRequest(
        int ListenedSeconds,
        bool IsCompleted);
}
