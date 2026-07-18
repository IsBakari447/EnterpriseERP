using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models;

public class Review
{
    public int Id { get; set; }

    [Required]
    [StringLength(140)]
    public string Title { get; set; } = "";

    [Required]
    [StringLength(3000)]
    public string Comment { get; set; } = "";

    [Range(1, 5)]
    public int Rating { get; set; } = 5;

    [StringLength(80)]
    public string Module { get; set; } = "EnterpriseERP";

    public bool IsApproved { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedByUserId { get; set; }

    [StringLength(160)]
    public string? CreatedByName { get; set; }

    [StringLength(160)]
    public string? CreatedByEmail { get; set; }
}
