namespace Spotify.Application.DTOs.Mood;

public sealed record MoodResponse(
    Guid Id,
    string Name,
    Guid? MoodImageId);