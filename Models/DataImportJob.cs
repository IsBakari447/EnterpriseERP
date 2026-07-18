using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models;

public class DataImportJob
{
    public int Id { get; set; }

    [Required]
    [StringLength(80)]
    public string Module { get; set; } = "";

    [Required]
    [StringLength(180)]
    public string FileName { get; set; } = "";

    public int RowsImported { get; set; }

    public int RowsFailed { get; set; }

    [StringLength(40)]
    public string Status { get; set; } = "Pending";

    [StringLength(2000)]
    public string? ErrorSummary { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [StringLength(160)]
    public string? CreatedBy { get; set; }
}
