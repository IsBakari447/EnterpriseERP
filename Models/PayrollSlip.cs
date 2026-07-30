using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models;

public class PayrollSlip
{
    public int Id { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    public Employee? Employee { get; set; }

    public string Period { get; set; } = DateTime.Today.ToString("yyyy-MM");

    public decimal GrossAmount { get; set; }

    public decimal NetAmount { get; set; }

    public string Status { get; set; } = "Brouillon";

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
