using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models;

public class Feedback
{
    public int Id { get; set; }

    [Required]
    [StringLength(140)]
    public string Title { get; set; } = "";

    [Required]
    [StringLength(3000)]
    public string Message { get; set; } = "";

    [StringLength(80)]
    public string Category { get; set; } = "General";

    [StringLength(40)]
    public string Status { get; set; } = "Nouveau";

    [StringLength(40)]
    public string Priority { get; set; } = "Normale";

    public bool IsPublic { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedByUserId { get; set; }

    [StringLength(160)]
    public string? CreatedByName { get; set; }

    [StringLength(160)]
    public string? CreatedByEmail { get; set; }

    [StringLength(2000)]
    public string? AdminResponse { get; set; }

    public DateTime? AdminResponseAt { get; set; }
}
