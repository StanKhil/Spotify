namespace Spotify.Application.DTOs.Auth
{
    public sealed record MeResponse(
        Guid Id, 
        string UserName, 
        string? Description,
        string Email,
        int FollowersCount,
        int FollowingCount,
        Spotify.Domain.Entities.Content.Author? author
    );

}
