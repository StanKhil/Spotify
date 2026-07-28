using System.Net;
using System.Net.Mail;
using Spotify.Application.Interfaces;
using Spotify.Infrastructure.Email;

namespace Spotify.Infrastructure.Services;

public sealed class DefaultEmailService : IEmailService
{
    private readonly EmailOptions _options;

    public DefaultEmailService(EmailOptions options)
    {
        _options = options;
    }

    public async Task SendAsync(
        string emailAddress,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.Host) ||
            string.IsNullOrWhiteSpace(_options.FromAddress))
        {
            throw new InvalidOperationException("Email SMTP settings are not configured.");
        }

        using var message = new MailMessage
        {
            From = new MailAddress(_options.FromAddress, _options.FromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };
        message.To.Add(emailAddress);

        using var client = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl = _options.EnableSsl,
            Credentials = new NetworkCredential(_options.UserName, _options.Password)
        };

        await client.SendMailAsync(message).WaitAsync(cancellationToken);
    }
}
