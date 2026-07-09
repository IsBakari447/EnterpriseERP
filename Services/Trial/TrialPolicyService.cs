using EnterpriseERP.Data;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseERP.Services.Trial;

public sealed class TrialPolicyService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public TrialPolicyService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<TrialStatus> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var firstUserCreatedAt = await _context.Users
            .OrderBy(u => u.CreatedAt)
            .Select(u => (DateTime?)u.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        var startedAt = DateTime.SpecifyKind(firstUserCreatedAt ?? now, DateTimeKind.Local).ToUniversalTime();
        var endsAt = startedAt.AddDays(TrialLimits.DurationDays);
        var isPaid = _configuration.GetValue("Trial:IsPaid", false);
        var isExpired = !isPaid && now >= endsAt;
        var daysRemaining = isPaid ? 0 : Math.Max(0, (int)Math.Ceiling((endsAt - now).TotalDays));

        var usersUsed = await _context.Users.CountAsync(cancellationToken);
        var invoicesUsed = await _context.Invoices.CountAsync(cancellationToken);
        var productsUsed = await _context.Products.CountAsync(cancellationToken);

        return new TrialStatus
        {
            TrialStartedAt = startedAt,
            TrialEndsAt = endsAt,
            DaysRemaining = daysRemaining,
            IsPaid = isPaid,
            IsExpired = isExpired,
            IsReadOnly = isExpired,
            PaymentRequired = isExpired,
            UsersUsed = usersUsed,
            InvoicesUsed = invoicesUsed,
            ProductsUsed = productsUsed,
            StatusLabel = isPaid
                ? "Abonnement actif"
                : isExpired
                    ? "Essai termine - lecture seule"
                    : $"Essai actif - {daysRemaining} jour(s) restant(s)",
            Suggestions = BuildSuggestions(isExpired, daysRemaining, usersUsed, invoicesUsed, productsUsed)
        };
    }

    public async Task<(bool Allowed, string Message)> CanCreateUserAsync(CancellationToken cancellationToken = default)
    {
        var status = await GetStatusAsync(cancellationToken);
        if (status.IsReadOnly)
            return (false, "Essai termine : lecture seule. Paiement obligatoire pour continuer.");

        if (!status.IsPaid && status.UsersUsed >= TrialLimits.MaxUsers)
            return (false, $"Limite de l'essai atteinte : {TrialLimits.MaxUsers} utilisateurs maximum.");

        return (true, "");
    }

    public async Task<(bool Allowed, string Message)> CanCreateInvoiceAsync(CancellationToken cancellationToken = default)
    {
        var status = await GetStatusAsync(cancellationToken);
        if (status.IsReadOnly)
            return (false, "Essai termine : lecture seule. Paiement obligatoire pour creer une facture.");

        if (!status.IsPaid && status.InvoicesUsed >= TrialLimits.MaxInvoices)
            return (false, $"Limite de l'essai atteinte : {TrialLimits.MaxInvoices} factures maximum.");

        return (true, "");
    }

    public async Task<(bool Allowed, string Message)> CanCreateProductAsync(CancellationToken cancellationToken = default)
    {
        var status = await GetStatusAsync(cancellationToken);
        if (status.IsReadOnly)
            return (false, "Essai termine : lecture seule. Paiement obligatoire pour creer un produit.");

        if (!status.IsPaid && status.ProductsUsed >= TrialLimits.MaxProducts)
            return (false, $"Limite de l'essai atteinte : {TrialLimits.MaxProducts} produits maximum.");

        return (true, "");
    }

    private static IReadOnlyList<string> BuildSuggestions(
        bool isExpired,
        int daysRemaining,
        int usersUsed,
        int invoicesUsed,
        int productsUsed)
    {
        var suggestions = new List<string>();

        if (isExpired)
        {
            suggestions.Add("Activez un abonnement pour reprendre les creations et modifications.");
            suggestions.Add($"Les donnees sont conservees {TrialLimits.DataRetentionDays} jours apres la fin de l'essai.");
            suggestions.Add("Exportez vos factures et produits importants avant la fin de conservation.");
            return suggestions;
        }

        if (daysRemaining <= 3)
            suggestions.Add("Votre essai se termine bientot : preparez le paiement pour eviter le passage en lecture seule.");

        if (usersUsed >= TrialLimits.MaxUsers - 1)
            suggestions.Add("Gardez les comptes d'essai pour les personnes essentielles afin de rester sous la limite de 3 utilisateurs.");

        if (invoicesUsed >= TrialLimits.MaxInvoices - 5)
            suggestions.Add("Vous approchez de la limite de 20 factures : finalisez vos tests puis passez a l'abonnement.");

        if (productsUsed >= TrialLimits.MaxProducts - 10)
            suggestions.Add("Importez le catalogue complet apres activation pour depasser la limite de 50 produits.");

        suggestions.Add("Testez les workflows principaux : clients, produits, factures, paiements, exports et mobile.");
        return suggestions;
    }
}
