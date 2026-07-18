using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models;

public class DataVersion
{
    public int Id { get; set; }

    [Required]
    [StringLength(80)]
    public string EntityType { get; set; } = "";

    [Required]
    [StringLength(80)]
    public string EntityId { get; set; } = "";

    public int VersionNumber { get; set; } = 1;

    [StringLength(4000)]
    public string SnapshotJson { get; set; } = "{}";

    [StringLength(200)]
    public string? ChangeSummary { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [StringLength(160)]
    public string? CreatedBy { get; set; }
}
