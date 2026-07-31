using Spotify.Application.DTOs.Plugin;

namespace Spotify.Application.Interfaces;

public interface IPluginService
{
    Task<IReadOnlyCollection<PluginResponse>> GetPluginsAsync(CancellationToken cancellationToken = default);
    Task<PluginResponse?> TogglePluginAsync(Guid id, TogglePluginRequest request, CancellationToken cancellationToken = default);
    Task<PluginResponse?> UpdatePluginSettingsAsync(Guid id, UpdatePluginSettingsRequest request, CancellationToken cancellationToken = default);
}