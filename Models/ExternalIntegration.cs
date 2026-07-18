using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models;

public class ExternalIntegration
{
    public int Id { get; set; }

    [Required]
    [StringLength(80)]
    public string Provider { get; set; } = "Zapier";

    [Required]
    [StringLength(140)]
    public string Name { get; set; } = "";

    [StringLength(500)]
    public string? WebhookUrl { get; set; }

    [StringLength(120)]
    public string? ApiKeyReference { get; set; }

    [StringLength(1000)]
    public string SettingsJson { get; set; } = "{}";

    public bool IsEnabled { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastSyncAt { get; set; }
}
