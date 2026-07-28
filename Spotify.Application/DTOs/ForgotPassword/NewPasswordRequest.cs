using System.ComponentModel.DataAnnotations;

namespace Spotify.Application.DTOs.ForgotPassword;

public sealed class NewPasswordRequest
{
    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; init; } = string.Empty;

    [Required]
    [StringLength(256, MinimumLength = 8)]
    public string Password { get; init; } = string.Empty;
}
