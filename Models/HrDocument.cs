using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models;

public class HrDocument
{
    public int Id { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    public Employee? Employee { get; set; }

    [Required]
    public string Title { get; set; } = "";

    public string DocumentType { get; set; } = "Contrat";

    public string FileUrl { get; set; } = "";

    public DateTime ExpirationDate { get; set; } = DateTime.Today.AddYears(1);

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
