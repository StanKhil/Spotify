namespace Spotify.Application.DTOs.Genre;

public sealed record GenreResponse(string Id, string Name, IReadOnlyCollection<string> TagIds);