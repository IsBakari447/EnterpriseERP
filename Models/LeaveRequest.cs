using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models;

public class LeaveRequest
{
    public int Id { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    public Employee? Employee { get; set; }

    [Required]
    public string Type { get; set; } = "Conges payes";

    public DateTime StartDate { get; set; } = DateTime.Today;

    public DateTime EndDate { get; set; } = DateTime.Today;

    public string Reason { get; set; } = "";

    public string Status { get; set; } = "En attente";

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
