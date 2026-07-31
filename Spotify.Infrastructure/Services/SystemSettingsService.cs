using Microsoft.EntityFrameworkCore;
using Spotify.Application.DTOs.SystemSettings;
using Spotify.Application.Interfaces;
using Spotify.Domain.Entities.Content;
using Spotify.Infrastructure.Persistance.Context;

namespace Spotify.Infrastructure.Services;

public sealed class SystemSettingsService : ISystemSettingsService
{
    private readonly ApplicationContext _context;

    public SystemSettingsService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<SystemSettingsResponse> GetSystemSettingsAsync(
        CancellationToken cancellationToken = default)
    {
        var settings = await _context.SystemSettings.ToListAsync(cancellationToken);
        return new SystemSettingsResponse(settings.ToDictionary(x => x.Key, x => x.Value));
    }

    public async Task<SystemSettingsResponse> UpdateSystemSettingsAsync(
        UpdateSystemSettingsRequest request, CancellationToken cancellationToken = default)
    {
        foreach (var (key, value) in request.Settings)
        {
            var existing = await _context.SystemSettings.FirstOrDefaultAsync(x => x.Key == key, cancellationToken);

            if (existing is null)
            {
                _context.SystemSettings.Add(new SystemSetting { Key = key, Value = value });
            }
            else
            {
                existing.Value = value;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return await GetSystemSettingsAsync(cancellationToken);
    }
}