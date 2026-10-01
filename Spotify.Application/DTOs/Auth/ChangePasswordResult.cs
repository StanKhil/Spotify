namespace Spotify.Application.DTOs.Auth;

public sealed record ChangePasswordResult(bool Succeeded, IReadOnlyCollection<string> Errors)
{
    public static ChangePasswordResult Success() =>
        new(true, Array.Empty<string>());

    public static ChangePasswordResult Failure(params string[] errors) =>
        new(false, errors);
}