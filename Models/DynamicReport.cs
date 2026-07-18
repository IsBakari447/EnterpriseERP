using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models;

public class DynamicReport
{
    public int Id { get; set; }

    [Required]
    [StringLength(140)]
    public string Name { get; set; } = "";

    [Required]
    [StringLength(80)]
    public string Module { get; set; } = "Dashboard";

    [StringLength(1000)]
    public string MetricsJson { get; set; } = "{}";

    [StringLength(1000)]
    public string FiltersJson { get; set; } = "{}";

    [StringLength(40)]
    public string RefreshMode { get; set; } = "Manual";

    public bool IsShared { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [StringLength(160)]
    public string? CreatedBy { get; set; }
}
