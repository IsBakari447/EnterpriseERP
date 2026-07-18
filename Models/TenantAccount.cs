using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models;

public class TenantAccount
{
    public int Id { get; set; }

    [Required]
    [StringLength(140)]
    public string Name { get; set; } = "";

    [Required]
    [StringLength(80)]
    public string Slug { get; set; } = "";

    [StringLength(80)]
    public string Plan { get; set; } = "Basic";

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
