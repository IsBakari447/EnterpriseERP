using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models;

public class AutomationRule
{
    public int Id { get; set; }

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = "";

    [Required]
    [StringLength(60)]
    public string Trigger { get; set; } = "LowStock";

    [Required]
    [StringLength(60)]
    public string Action { get; set; } = "CreateNotification";

    [StringLength(1000)]
    public string ConditionsJson { get; set; } = "{}";

    [StringLength(1000)]
    public string ActionPayloadJson { get; set; } = "{}";

    public bool IsEnabled { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastRunAt { get; set; }
}
