namespace Spotify.Application.DTOs.Jamendo;

// Jamendo calls this resource "artist"; Spotify exposes it as an Author.
public sealed record JamendoAuthorDto(
    string Id,
    string Name,
    string ImageUrl,
    string WebsiteUrl,
    string ShortUrl,
    string ShareUrl,
    DateTime? JoinedAt);
