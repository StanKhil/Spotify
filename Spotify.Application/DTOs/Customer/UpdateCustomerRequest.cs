using System.ComponentModel.DataAnnotations;

namespace Spotify.Application.DTOs.Customer;

public sealed class UpdateCustomerRequest
{
    [Required]
    [MaxLength(100)]
    public string UserName { get; init; } = string.Empty;

    [Required]
    public Guid CountryId { get; init; }

    [Required]
    public Guid CityId { get; init; }

    [Required]
    public DateOnly Birthdate { get; init; }
}