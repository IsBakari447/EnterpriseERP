using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models;

public class SocialLike
{
    public int Id { get; set; }

    [Required]
    [StringLength(80)]
    public string TargetType { get; set; } = "";

    [Required]
    [StringLength(120)]
    public string TargetId { get; set; } = "";

    public int? UserId { get; set; }

    [StringLength(160)]
    public string UserName { get; set; } = "";

    [StringLength(160)]
    public string UserEmail { get; set; } = "";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
