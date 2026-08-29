using Spotify.Application.Interfaces;
using Spotify.Domain.Entities.Content;
using Spotify.Domain.Enumerations;
using Spotify.Infrastructure.Playback;


namespace Spotify.Infrastructure.Services
{
    public sealed class AudioUrlResolver : IAudioUrlResolver
    {
        private readonly IJamendoService _jamendoService;
        private readonly ILocalPlaybackUrlService _localPlaybackUrlService;
        private readonly PlaybackOptions _playbackOptions;

        public AudioUrlResolver(
            IJamendoService jamendoService,
            ILocalPlaybackUrlService localPlaybackUrlService,
            PlaybackOptions playbackOptions)
        {
            _jamendoService = jamendoService;
            _localPlaybackUrlService = localPlaybackUrlService;
            _playbackOptions = playbackOptions;
        }

        public async Task<string?> ResolveAsync(
            AudioItem audioItem,
            CancellationToken cancellationToken = default)
        {
            if (audioItem.Provider == AudioProvider.Jamendo)
            {
                if (string.IsNullOrWhiteSpace(audioItem.ExternalContentId))
                {
                    return null;
                }

                return await _jamendoService.GetTrackStreamUrlAsync(
                    audioItem.ExternalContentId,
                    cancellationToken);
            }

            if (string.IsNullOrWhiteSpace(audioItem.StorageKey))
            {
                return null;
            }

            var expiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(
                _playbackOptions.LocalUrlLifetimeMinutes);

            return _localPlaybackUrlService.CreateStreamUrl(
                audioItem.Id,
                expiresAtUtc);
        }
    }
}
