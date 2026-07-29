
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Spotify.Application.Interfaces;
using Spotify.Infrastructure.Authentication;
using Spotify.Infrastructure.Persistance.Context;
using Spotify.Infrastructure.Services;
using System.Text;
using Spotify.Domain.Entities.User;
using Spotify.Domain.Entities.Security;

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
                })
                .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
                {
                    options.ClientId = builder.Configuration["Authentication:Google:ClientId"]
                        ?? throw new InvalidOperationException("Authentication:Google:ClientId is missing.");
                    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]
                        ?? throw new InvalidOperationException("Authentication:Google:ClientSecret is missing.");
                    options.CallbackPath = "/signin-google";
                    options.SignInScheme = IdentityConstants.ExternalScheme;
                });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowFrontend");

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
