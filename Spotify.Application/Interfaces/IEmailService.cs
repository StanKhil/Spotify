namespace Spotify.Application.Interfaces;

public interface IEmailService
{
    Task SendAsync(
        string emailAddress,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default);
}
