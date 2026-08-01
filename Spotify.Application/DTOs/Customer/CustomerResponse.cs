namespace Spotify.Application.DTOs.Customer;

public sealed record CustomerResponse(
    Guid Id,
    string Email,
    string UserName,
    Guid CountryId,
    Guid CityId,
    DateTime Birthdate,
    bool IsAdult,
    DateTime RegisteredAt,
    DateTime? DeletedAt);