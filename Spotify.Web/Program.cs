
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Spotify.Application.Interfaces;
using Spotify.Domain.Entities.Content;
using Spotify.Infrastructure.Authentication;
using Spotify.Infrastructure.Email;
using Spotify.Infrastructure.Persistance.Context;
using Spotify.Infrastructure.Services;
using Spotify.Infrastructure.Storage;
using System.Text;

namespace Spotify
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            // Database
            builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("LocalDatabase")));
            builder.Services.AddIdentity<ApplicationUser, UserRole>().AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<IGenreService, GenreService>();
            builder.Services.AddScoped<ITagService, TagService>();
            builder.Services.AddScoped<IMoodService, MoodService>();
            builder.Services.AddScoped<IAlbumService, AlbumService>();
            builder.Services.AddScoped<IPodcastService, PodcastService>();
            builder.Services.AddScoped<IEpisodeService, EpisodeService>();
            builder.Services.AddScoped<IPlaylistService, PlaylistService>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<IAudiobookService, AudiobookService>();
            builder.Services.AddScoped<IAuthorService, AuthorService>();
            builder.Services.AddScoped<ITrackService, TrackService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();
            builder.Services.AddScoped<IPluginService, PluginService>();
            builder.Services.AddScoped<ISystemSettingsService, SystemSettingsService>();
            builder.Services.AddScoped<IEmailService, DefaultEmailService>();
            builder.Services.AddMemoryCache();

            var emailOptions = builder.Configuration
                .GetSection(EmailOptions.SectionName)
                .Get<EmailOptions>()
                ?? new EmailOptions();
            builder.Services.AddSingleton(emailOptions);

            var jwtOptions = builder.Configuration
                .GetRequiredSection(JwtOptions.SectionName)
                .Get<JwtOptions>()
                ?? throw new InvalidOperationException("JWT configuration is missing.");

            if (string.IsNullOrWhiteSpace(jwtOptions.Key) || jwtOptions.Key.Length < 32)
            {
                throw new InvalidOperationException("Jwt:Key must contain at least 32 characters.");
            }

            builder.Services.AddSingleton(jwtOptions);
            builder.Services.AddSingleton<JwtTokenGenerator>();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                });
            });

            var app = builder.Build();

            builder.Services.AddSingleton<IFileStorageService>(
            new LocalFileStorageService(builder.Environment.WebRootPath ?? Path.Combine(builder.Environment.ContentRootPath, "wwwroot")));
            builder.Services.AddScoped<IMediaService, MediaService>();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowFrontend");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();

            app.MapControllers();

            app.Run();
        }
    }
}
