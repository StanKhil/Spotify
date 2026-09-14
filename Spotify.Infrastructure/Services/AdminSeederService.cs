using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Spotify.Application.Interfaces;
using Spotify.Domain.Entities.User;
using Spotify.Domain.Enumerations;
using Spotify.Infrastructure.Persistance.Context;

namespace Spotify.Infrastructure.Services;

public sealed class AdminSeederService : IAdminSeederService
{
    private const string AdminRoleName = "Admin";

    private readonly ApplicationContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<UserRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AdminSeederService(
        ApplicationContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<UserRole> roleManager,
        IConfiguration configuration)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    public async Task SeedInitialAdminAsync(CancellationToken cancellationToken = default)
    {
        var email = _configuration["InitialAdmin:Email"];
        var userName = _configuration["InitialAdmin:UserName"];
        var password = _configuration["InitialAdmin:Password"];

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(userName) ||
            string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        if (!await _roleManager.RoleExistsAsync(AdminRoleName))
        {
            await _roleManager.CreateAsync(new UserRole
            {
                Id = Guid.NewGuid(),
                Name = AdminRoleName,
                Description = "Administrator role",
                CanRead = true,
                CanCreate = true,
                CanUpdate = true,
                CanDelete = true
            });
        }

        var existingAdmins = await _userManager.GetUsersInRoleAsync(AdminRoleName);
        if (existingAdmins.Count > 0)
        {
            return;
        }

        var defaultSubscriptionId = await _context.Subscriptions
            .Where(s => s.Name == "Default" || s.Name == "Basic")
            .Select(s => s.Id)
            .FirstOrDefaultAsync(cancellationToken);

        var settings = new Settings
        {
            Id = Guid.NewGuid(),
            Language = Language.English
        };

        _context.Settings.Add(settings);
        await _context.SaveChangesAsync(cancellationToken);

        var admin = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            UserName = userName,
            EmailConfirmed = true,
            SubscriptionId = defaultSubscriptionId,
            SettingsId = settings.Id
        };

        var result = await _userManager.CreateAsync(admin, password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(admin, AdminRoleName);
        }
    }
}