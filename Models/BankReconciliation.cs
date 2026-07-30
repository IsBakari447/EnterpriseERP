using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models;

public class BankReconciliation
{
    public int Id { get; set; }

    [Required]
    public string BankAccount { get; set; } = "";

    public DateTime StatementDate { get; set; } = DateTime.Today;

    public decimal StatementBalance { get; set; }

    public decimal ErpBalance { get; set; }

    public string Status { get; set; } = "A verifier";

    public string Notes { get; set; } = "";
}
