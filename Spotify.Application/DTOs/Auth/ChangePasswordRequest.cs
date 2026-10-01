using System.ComponentModel.DataAnnotations;

namespace Spotify.Application.DTOs.Auth;

public sealed class ChangePasswordRequest
{
    [Required]
    [StringLength(256)]
    public string CurrentPassword { get; init; } = string.Empty;

    [Required]
    [StringLength(256, MinimumLength = 8)]
    public string NewPassword { get; init; } = string.Empty;
}