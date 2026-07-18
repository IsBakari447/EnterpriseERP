using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models;

public class MarketplaceExtension
{
    public int Id { get; set; }

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = "";

    [Required]
    [StringLength(80)]
    public string Category { get; set; } = "Productivity";

    [StringLength(500)]
    public string Description { get; set; } = "";

    [StringLength(200)]
    public string InstallUrl { get; set; } = "";

    public bool IsInstalled { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
