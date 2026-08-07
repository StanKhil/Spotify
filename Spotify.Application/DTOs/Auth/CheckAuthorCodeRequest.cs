namespace Spotify.Application.DTOs.Auth
{
    public sealed record CheckAuthorCodeRequest(
        string UserEmail,
        string ActivationCode
    );
}
