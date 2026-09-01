namespace Spotify.Application.DTOs.Jamendo;

public sealed record JamendoAuthorAlbumsDto(
    JamendoAuthorDto Author,
    IReadOnlyCollection<JamendoAlbumDto> Albums);
