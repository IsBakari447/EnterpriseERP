using EnterpriseERP.Data;
using EnterpriseERP.Helpers;
using EnterpriseERP.Services.Email;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseERP.Services;

public sealed class PasswordResetService
{
    private static readonly TimeSpan CodeLifetime = TimeSpan.FromMinutes(20);
    private static readonly TimeSpan RequestWindow = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan RequestLockDuration = TimeSpan.FromMinutes(15);
    private const int MaxRequestsPerWindow = 3;

    private readonly ApplicationDbContext _context;
    private readonly PasswordResetTokenService _tokenService;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<PasswordResetService> _logger;

    public PasswordResetService(
        ApplicationDbContext context,
        PasswordResetTokenService tokenService,
        IEmailSender emailSender,
        ILogger<PasswordResetService> logger)
    {
        _context = context;
        _tokenService = tokenService;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task<(bool Success, string Message)> RequestResetAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            return (false, "Veuillez saisir votre adresse e-mail.");

        var normalizedEmail = email.Trim().ToUpperInvariant();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToUpper() == normalizedEmail, cancellationToken);
        if (user == null || !user.IsActive || !user.IsApproved)
            return GenericRequestResponse();

        var now = DateTime.UtcNow;
        if (user.PasswordResetLockedUntil.HasValue && user.PasswordResetLockedUntil.Value > now)
            return (false, "Trop de demandes. Veuillez reessayer plus tard.");

        if (!user.PasswordResetRequestWindowStartedAt.HasValue ||
            user.PasswordResetRequestWindowStartedAt.Value.Add(RequestWindow) <= now)
        {
            user.PasswordResetRequestWindowStartedAt = now;
            user.PasswordResetRequestCount = 0;
        }

        user.PasswordResetRequestCount += 1;
        if (user.PasswordResetRequestCount > MaxRequestsPerWindow)
        {
            user.PasswordResetLockedUntil = now.Add(RequestLockDuration);
            await _context.SaveChangesAsync(cancellationToken);
            return (false, "Trop de demandes. Veuillez reessayer plus tard.");
        }

        var code = _tokenService.GenerateCode();
        user.PasswordResetTokenHash = _tokenService.HashCode(code);
        user.PasswordResetTokenExpiresAt = now.Add(CodeLifetime);
        user.PasswordResetTokenUsedAt = null;
        user.UpdatedAt = now;

        var sent = await _emailSender.SendAsync(new EmailMessage
        {
            To = user.Email,
            Subject = "Code de verification EnterpriseERP",
            TextBody = $"Votre code de verification EnterpriseERP est : {code}. Il expire dans 20 minutes.",
            HtmlBody = $"""
                <p>Bonjour {System.Net.WebUtility.HtmlEncode(user.FullName)},</p>
                <p>Votre code de verification EnterpriseERP est :</p>
                <p style="font-size:28px;font-weight:800;letter-spacing:6px;">{code}</p>
                <p>Ce code expire dans 20 minutes et ne peut etre utilise qu'une seule fois.</p>
                """
        }, cancellationToken);

        if (!sent)
        {
            user.PasswordResetTokenHash = null;
            user.PasswordResetTokenExpiresAt = null;
            user.PasswordResetTokenUsedAt = null;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogWarning("Password reset code was not delivered for {Email}.", user.Email);

            return (false, "Le code n'a pas pu etre envoye. Verifiez la configuration SMTP ou utilisez un mot de passe d'application Gmail.");
        }

        await _context.SaveChangesAsync(cancellationToken);
        return GenericRequestResponse();
    }

    public async Task<(bool Success, string Message)> ResetPasswordAsync(
        string email,
        string code,
        string password,
        string confirmPassword,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(code) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(confirmPassword))
        {
            return (false, "Veuillez remplir tous les champs.");
        }

        if (password != confirmPassword)
            return (false, "Les mots de passe ne correspondent pas.");

        if (password.Length < 8)
            return (false, "Le nouveau mot de passe doit contenir au moins 8 caracteres.");

        var normalizedEmail = email.Trim().ToUpperInvariant();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToUpper() == normalizedEmail, cancellationToken);
        if (user == null ||
            string.IsNullOrWhiteSpace(user.PasswordResetTokenHash) ||
            user.PasswordResetTokenExpiresAt == null ||
            user.PasswordResetTokenUsedAt != null ||
            user.PasswordResetTokenExpiresAt <= DateTime.UtcNow ||
            !_tokenService.Matches(code, user.PasswordResetTokenHash))
        {
            return (false, "Le code de verification est invalide ou expire.");
        }

        user.PasswordHash = PasswordHelper.HashPassword(password);
        user.PasswordResetTokenUsedAt = DateTime.UtcNow;
        user.PasswordResetTokenHash = null;
        user.PasswordResetTokenExpiresAt = null;
        user.PasswordResetRequestCount = 0;
        user.PasswordResetRequestWindowStartedAt = null;
        user.PasswordResetLockedUntil = null;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return (true, "Votre mot de passe a ete mis a jour.");
    }

    private static (bool Success, string Message) GenericRequestResponse()
    {
        return (true, "Si un compte existe avec cet e-mail, un code de verification a ete envoye.");
    }
}
