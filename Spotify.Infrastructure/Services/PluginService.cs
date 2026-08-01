using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.Plugin;
using Spotify.Application.Interfaces;
using Spotify.Infrastructure.Persistance.Context;

namespace Spotify.Infrastructure.Services;

public sealed class PluginService : IPluginService
{
    private readonly ApplicationContext _context;

    public PluginService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<PluginResponse>> GetPluginsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Plugins
            .OrderBy(x => x.Name)
            .Select(x => new PluginResponse(x.Id, x.Name, x.IsEnabled, x.SettingsJson))
            .ToListAsync(cancellationToken);
    }

    public async Task<PluginResponse?> TogglePluginAsync(
        Guid id, TogglePluginRequest request, CancellationToken cancellationToken = default)
    {
        var plugin = await _context.Plugins.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (plugin is null) return null;

        plugin.IsEnabled = request.IsEnabled;
        await _context.SaveChangesAsync(cancellationToken);

        return new PluginResponse(plugin.Id, plugin.Name, plugin.IsEnabled, plugin.SettingsJson);
    }

    public async Task<PluginResponse?> UpdatePluginSettingsAsync(
        Guid id, UpdatePluginSettingsRequest request, CancellationToken cancellationToken = default)
    {
        var plugin = await _context.Plugins.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (plugin is null) return null;

        plugin.SettingsJson = request.SettingsJson;
        await _context.SaveChangesAsync(cancellationToken);

        return new PluginResponse(plugin.Id, plugin.Name, plugin.IsEnabled, plugin.SettingsJson);
    }
}