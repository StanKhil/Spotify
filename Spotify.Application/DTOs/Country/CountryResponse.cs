using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Application.DTOs.Country
{
    public sealed record CountryResponse(
        Guid Id,
        string Name
    );
}
