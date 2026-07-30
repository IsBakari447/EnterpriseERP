using EnterpriseERP.Attributes;
using EnterpriseERP.Data;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseERP.Controllers;

public class AiStudioController : Controller
{
    private readonly ApplicationDbContext _context;

    public AiStudioController(ApplicationDbContext context)
    {
        _context = context;
    }

    [RequirePermission("IA", "Voir")]
    public IActionResult Index()
    {
        var revenue = _context.Invoices
            .Select(i => i.TotalAmount)
            .AsEnumerable()
            .DefaultIfEmpty(0m)
            .Sum();

        var paid = _context.Payments
            .Select(p => p.Amount)
            .AsEnumerable()
            .DefaultIfEmpty(0m)
            .Sum();

        var expenses = _context.Expenses
            .Select(e => e.Amount)
            .AsEnumerable()
            .DefaultIfEmpty(0m)
            .Sum();
        var lowStock = _context.Products.Count(p => p.Quantity <= 5);

        ViewBag.SalesPrediction = revenue > 0 ? revenue * 1.08m : 0;
        ViewBag.CashPrediction = paid - expenses;
        ViewBag.StockRisk = lowStock;
        ViewBag.Features = new[]
        {
            "InvoiceTranslation",
            "ReportSummary",
            "ClientEmailGeneration",
            "PriorityAnalysis",
            "SalesForecastFeature",
            "StockForecastFeature",
            "PushNotifications",
            "ProductQrScan",
            "DigitalQuoteInvoiceSignature",
            "SecureTokenlessLogs"
        };

        return View();
    }
}
