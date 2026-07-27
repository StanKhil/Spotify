namespace Spotify.Application.DTOs.Auth;

public sealed record LoginResult(
    bool Succeeded,
    AuthenticationResponse? Authentication,
    IReadOnlyCollection<string> Errors)
{
    public static LoginResult Success(AuthenticationResponse authentication) =>
        new(true, authentication, []);

    public static LoginResult Failure(params string[] errors) =>
        new(false, null, errors);
}
