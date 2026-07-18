using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models;

public class CollaborationComment
{
    public int Id { get; set; }

    [Required]
    [StringLength(80)]
    public string EntityType { get; set; } = "";

    [Required]
    [StringLength(80)]
    public string EntityId { get; set; } = "";

    [Required]
    [StringLength(2000)]
    public string Body { get; set; } = "";

    [StringLength(500)]
    public string Mentions { get; set; } = "";

    public bool IsSharedExternally { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [StringLength(160)]
    public string? CreatedBy { get; set; }
}
