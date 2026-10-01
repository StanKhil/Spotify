using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;
using Spotify.Application.Interfaces;
using Spotify.Domain.Entities.Security;

namespace Spotify.Infrastructure.Services;

public sealed class DefaultEmailService : IEmailService
{
    private readonly EmailOptions _options;
    private readonly ILogger<DefaultEmailService> _logger;

    public DefaultEmailService(EmailOptions options, ILogger<DefaultEmailService> logger)
    {
        _options = options;
        _logger = logger;
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

        var userName = _options.UserName?.Trim() ?? string.Empty;
        var password = (_options.Password ?? string.Empty).Replace(" ", string.Empty);

        if (userName.Length == 0 || password.Length == 0)
        {
            _logger.LogWarning(
                "SMTP credentials are empty (UserName length: {UserLen}, Password length: {PassLen}). Check appsettings.Development.json, user-secrets and environment variables.",
                userName.Length, password.Length);
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_options.FromName, _options.FromAddress));
        message.To.Add(MailboxAddress.Parse(emailAddress));
        message.Subject = subject;
        message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        var socketOptions = _options.Port == 465
            ? SecureSocketOptions.SslOnConnect
            : _options.EnableSsl
                ? SecureSocketOptions.StartTls
                : SecureSocketOptions.Auto;

        using var client = new SmtpClient();

        await client.ConnectAsync(_options.Host, _options.Port, socketOptions, cancellationToken);

        if (userName.Length > 0)
        {
            await client.AuthenticateAsync(userName, password, cancellationToken);
        }

        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}