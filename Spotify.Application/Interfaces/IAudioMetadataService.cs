namespace Spotify.Application.Interfaces
{
    public interface IAudioMetadataService
    {
        Task<int> GetDurationSecondsAsync(
            string storageKey,
            CancellationToken cancellationToken = default);
    }
}
