namespace Spotify.Application.DTOs.UserProfile;

public sealed record UserProfileStatisticsResponse(
    int TotalSubsrcibesCount,
    int TotalContentCount,
    long TotalListenersCount);


