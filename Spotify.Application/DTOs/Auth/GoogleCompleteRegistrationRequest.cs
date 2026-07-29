using System.ComponentModel.DataAnnotations;

namespace Spotify.Application.DTOs.Auth;

public sealed class GoogleCompleteRegistrationRequest
{
    [Required]
    public string RegistrationToken { get; init; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; init; } = string.Empty;

    [Required]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; init; } = string.Empty;

    public Guid CountryId { get; init; }
    public Guid CityId { get; init; }
    public Guid SubscriptionId { get; init; }
    public DateOnly Birthdate { get; init; }
}
