using Spotify.Application.DTOs.Auth;

namespace Spotify.Application.Interfaces;

public interface IAuthenticationService
{
    Task<RegisterResult> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default);

    Task<LoginResult> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default);
}
