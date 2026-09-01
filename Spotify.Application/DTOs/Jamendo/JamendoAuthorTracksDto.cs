namespace Spotify.Application.DTOs.Jamendo;

public sealed record JamendoAuthorTracksDto(
    JamendoAuthorDto Author,
    IReadOnlyCollection<JamendoTrackDto> Tracks);
