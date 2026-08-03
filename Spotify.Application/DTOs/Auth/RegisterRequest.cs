using System.ComponentModel.DataAnnotations;

namespace Spotify.Application.DTOs.Auth;

public sealed class RegisterRequest
{
    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; init; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string UserName { get; init; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; init; } = string.Empty;

    public Guid CountryId { get; init; }
    public Guid CityId { get; init; }
    public Boolean IsAuthor { get; init; }
    public DateOnly Birthdate { get; init; }
}
