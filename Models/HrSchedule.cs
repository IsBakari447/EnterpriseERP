using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models;

public class HrSchedule
{
    public int Id { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    public Employee? Employee { get; set; }

    public DateTime WorkDate { get; set; } = DateTime.Today;

    public string StartTime { get; set; } = "08:00";

    public string EndTime { get; set; } = "17:00";

    public string Notes { get; set; } = "";
}
