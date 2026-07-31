using Spotify.Application.DTOs.SystemSettings;

namespace Spotify.Application.Interfaces;

public interface ISystemSettingsService
{
    Task<SystemSettingsResponse> GetSystemSettingsAsync(CancellationToken cancellationToken = default);
    Task<SystemSettingsResponse> UpdateSystemSettingsAsync(UpdateSystemSettingsRequest request, CancellationToken cancellationToken = default);
}