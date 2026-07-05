using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Portfolio.Application.Common.Interfaces;

namespace Portfolio.Infrastructure.Services.Email;

// ── Settings ─────────────────────────────────────────────────────────────────

public sealed class SmtpSettings
{
    public string Host        { get; init; } = "localhost";
    public int    Port        { get; init; } = 587;
    public string Username    { get; init; } = string.Empty;
    public string Password    { get; init; } = string.Empty;
    public string FromEmail   { get; init; } = string.Empty;
    public string FromName    { get; init; } = "Portfolio";
    public string OwnerEmail  { get; init; } = string.Empty;   // where to notify you
    public bool   UseSsl      { get; init; } = false;
}

// ── Service ───────────────────────────────────────────────────────────────────

/// <summary>
/// SMTP email service using MailKit.
/// Works locally with MailHog (port 1025) and in production with any SMTP provider
/// (Gmail, Outlook, SMTP2GO) — just swap appsettings values.
/// </summary>
public sealed class SmtpEmailService(
    IOptions<SmtpSettings> options,
    ILogger<SmtpEmailService> logger) : IEmailService
{
    private readonly SmtpSettings _settings = options.Value;

    public async Task SendContactNotificationAsync(
        string senderName, string senderEmail,
        string subject, string message, CancellationToken ct = default)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
        email.To.Add(new MailboxAddress("Portfolio Owner", _settings.OwnerEmail));
        email.Subject = $"[Portfolio Contact] {subject}";

        var body = new BodyBuilder
        {
            HtmlBody = $"""
                <h2>New Contact Message</h2>
                <p><strong>From:</strong> {senderName} ({senderEmail})</p>
                <p><strong>Subject:</strong> {subject}</p>
                <hr />
                <p>{message.Replace("\n", "<br/>")}</p>
                """,
            TextBody = $"From: {senderName} ({senderEmail})\nSubject: {subject}\n\n{message}",
        };

        email.Body = body.ToMessageBody();
        await SendAsync(email, ct);
        logger.LogInformation("Contact notification sent for {SenderEmail}", senderEmail);
    }

    public async Task SendContactConfirmationAsync(
        string toEmail, string toName, CancellationToken ct = default)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
        email.To.Add(new MailboxAddress(toName, toEmail));
        email.Subject = "Thanks for reaching out!";

        var body = new BodyBuilder
        {
            HtmlBody = $"""
                <h2>Hi {toName},</h2>
                <p>Thanks for your message! I've received it and will get back to you within 24 hours.</p>
                <p>In the meantime, feel free to check out my projects on
                   <a href="https://github.com/yourname">GitHub</a>.</p>
                <br/>
                <p>Best,<br/>Your Name</p>
                """,
            TextBody = $"Hi {toName},\n\nThanks for your message! I'll get back to you within 24 hours.\n\nBest,\nYour Name",
        };

        email.Body = body.ToMessageBody();
        await SendAsync(email, ct);
        logger.LogInformation("Confirmation email sent to {ToEmail}", toEmail);
    }

    // ── Internal sender ───────────────────────────────────────────────────────
    private async Task SendAsync(MimeMessage email, CancellationToken ct)
    {
        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(
                _settings.Host,
                _settings.Port,
                _settings.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTlsWhenAvailable,
                ct);

            if (!string.IsNullOrEmpty(_settings.Username))
                await client.AuthenticateAsync(_settings.Username, _settings.Password, ct);

            await client.SendAsync(email, ct);
        }
        finally
        {
            await client.DisconnectAsync(true, ct);
        }
    }
}
