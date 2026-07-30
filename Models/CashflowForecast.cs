using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models;

public class CashflowForecast
{
    public int Id { get; set; }

    [Required]
    public string Period { get; set; } = DateTime.Today.ToString("yyyy-MM");

    public decimal ExpectedInflow { get; set; }

    public decimal ExpectedOutflow { get; set; }

    public string Scenario { get; set; } = "Realiste";

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
