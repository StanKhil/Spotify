namespace Spotify.Application.DTOs.Auth;

public sealed record GoogleSignInResult(
    bool Succeeded,
    bool RequiresRegistration,
    AuthenticationResponse? Authentication,
    string? RegistrationToken,
    IReadOnlyCollection<string> Errors)
{
    public static GoogleSignInResult Authenticated(AuthenticationResponse authentication) =>
        new(true, false, authentication, null, []);

    public static GoogleSignInResult RegistrationRequired(string registrationToken) =>
        new(true, true, null, registrationToken, []);

    public static GoogleSignInResult Failure(params string[] errors) =>
        new(false, false, null, null, errors);
}
