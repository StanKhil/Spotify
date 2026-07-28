using Spotify.Application.DTOs.Auth;
using Spotify.Application.DTOs.ForgotPassword;

namespace Spotify.Application.Interfaces;

public interface IAuthenticationService
{
    Task<RegisterResult> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default);

    Task<LoginResult> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default);

    Task<CheckEmailResult> CheckEmailAsync(
        CheckEmailRequest request,
        CancellationToken cancellationToken = default);

    Task<CheckCodeResult> CheckCodeAsync(
        CheckCodeRequest request,
        CancellationToken cancellationToken = default);

    Task<NewPasswordResult> NewPasswordAsync(
        NewPasswordRequest request,
        CancellationToken cancellationToken = default);
}
