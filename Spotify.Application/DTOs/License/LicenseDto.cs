using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Application.DTOs.License
{
    public sealed record LicenseDto(
        Guid Id,
        string UserName,
        string UserEmail,
        string ActivationKey
        ); 
}
