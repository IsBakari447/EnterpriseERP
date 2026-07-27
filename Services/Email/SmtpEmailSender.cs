using System.Net;
using System.Net.Mail;

namespace EnterpriseERP.Services.Email;

public sealed class SmtpEmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IConfiguration configuration, ILogger<SmtpEmailSender> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        var host = _configuration["Email:Smtp:Host"]
            ?? Environment.GetEnvironmentVariable("ENTERPRISEERP_SMTP_HOST")
            ?? Environment.GetEnvironmentVariable("SMTP_HOST");
        var from = _configuration["Email:From"]
            ?? Environment.GetEnvironmentVariable("ENTERPRISEERP_EMAIL_FROM")
            ?? Environment.GetEnvironmentVariable("EMAIL_FROM");

        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(from))
        {
            _logger.LogWarning("SMTP is not configured. Password reset email was not sent for {Email}.", message.To);
            return;
        }

        using var mail = new MailMessage
        {
            From = new MailAddress(from, _configuration["Email:SenderName"] ?? "EnterpriseERP"),
            Subject = message.Subject,
            Body = string.IsNullOrWhiteSpace(message.HtmlBody) ? message.TextBody : message.HtmlBody,
            IsBodyHtml = !string.IsNullOrWhiteSpace(message.HtmlBody)
        };
        mail.To.Add(message.To);

        using var smtp = new SmtpClient(host)
        {
            Port = int.TryParse(_configuration["Email:Smtp:Port"] ?? Environment.GetEnvironmentVariable("SMTP_PORT"), out var port) ? port : 587,
            EnableSsl = bool.TryParse(_configuration["Email:Smtp:EnableSsl"] ?? Environment.GetEnvironmentVariable("SMTP_SECURE"), out var ssl) ? ssl : true
        };

        var username = _configuration["Email:Smtp:Username"]
            ?? Environment.GetEnvironmentVariable("ENTERPRISEERP_SMTP_USERNAME")
            ?? Environment.GetEnvironmentVariable("SMTP_USERNAME");
        var password = _configuration["Email:Smtp:Password"]
            ?? Environment.GetEnvironmentVariable("ENTERPRISEERP_SMTP_PASSWORD")
            ?? Environment.GetEnvironmentVariable("SMTP_PASSWORD");

        if (!string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning("SMTP password is missing. Password reset email was not sent for {Email}.", message.To);
            return;
        }

        if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            smtp.Credentials = new NetworkCredential(username, password);

        await smtp.SendMailAsync(mail, cancellationToken);
    }
}
