namespace EnterpriseERP.Services.Trial;

public sealed class TrialStatus
{
    public string PlanName { get; init; } = "Essai gratuit";
    public int DurationDays { get; init; } = TrialLimits.DurationDays;
    public string Role { get; init; } = TrialLimits.TrialRole;
    public int MaxUsers { get; init; } = TrialLimits.MaxUsers;
    public int MaxInvoices { get; init; } = TrialLimits.MaxInvoices;
    public int MaxProducts { get; init; } = TrialLimits.MaxProducts;
    public int DataRetentionDays { get; init; } = TrialLimits.DataRetentionDays;
    public DateTime TrialStartedAt { get; init; }
    public DateTime TrialEndsAt { get; init; }
    public int DaysRemaining { get; init; }
    public bool IsPaid { get; init; }
    public bool IsExpired { get; init; }
    public bool IsReadOnly { get; init; }
    public bool PaymentRequired { get; init; }
    public int UsersUsed { get; init; }
    public int InvoicesUsed { get; init; }
    public int ProductsUsed { get; init; }
    public string StatusLabel { get; init; } = "";
    public IReadOnlyList<string> Suggestions { get; init; } = Array.Empty<string>();
}
