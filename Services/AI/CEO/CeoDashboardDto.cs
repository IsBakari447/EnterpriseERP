namespace EnterpriseERP.Services.AI.CEO;

public class CeoDashboardDto
{
    public int HealthScore { get; set; }
    public decimal Revenue { get; set; }
    public decimal Payments { get; set; }
    public decimal Expenses { get; set; }
    public decimal Profit { get; set; }
    public decimal MarginRate { get; set; }
    public decimal CollectionRate { get; set; }
    public decimal ForecastRevenue30Days { get; set; }
    public decimal ForecastExpenses30Days { get; set; }
    public int CashRunwayDays { get; set; }

    public int Clients { get; set; }
    public int Products { get; set; }
    public int Invoices { get; set; }
    public int UnpaidInvoices { get; set; }
    public int LowStockProducts { get; set; }
    public int PresentEmployees { get; set; }

    public string ExecutiveSummary { get; set; } = "";
    public string PriorityRecommendation { get; set; } = "";
    public string RiskLevel { get; set; } = "Stable";

    public List<string> Alerts { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
    public List<string> StrategicActions { get; set; } = new();
}
