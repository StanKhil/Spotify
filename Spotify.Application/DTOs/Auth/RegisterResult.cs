namespace Spotify.Application.DTOs.Auth;

public sealed record RegisterResult(
    bool Succeeded,
    RegisterResponse? User,
    IReadOnlyCollection<string> Errors)
{
    public static RegisterResult Success(RegisterResponse user) =>
        new(true, user, []);

    public static RegisterResult Failure(params string[] errors) =>
        new(false, null, errors);
}
