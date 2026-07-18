using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models;

public class CustomFieldDefinition
{
    public int Id { get; set; }

    [Required]
    [StringLength(80)]
    public string EntityType { get; set; } = "Client";

    [Required]
    [StringLength(80)]
    public string FieldKey { get; set; } = "";

    [Required]
    [StringLength(120)]
    public string Label { get; set; } = "";

    [StringLength(40)]
    public string FieldType { get; set; } = "Text";

    public bool IsRequired { get; set; }

    [StringLength(1000)]
    public string OptionsJson { get; set; } = "[]";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
