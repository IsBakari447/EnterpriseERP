using EnterpriseERP.Attributes;
using EnterpriseERP.Data;
using EnterpriseERP.Models;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseERP.Controllers;

public class FinanceAdvancedController : Controller
{
    private readonly ApplicationDbContext _context;

    public FinanceAdvancedController(ApplicationDbContext context)
    {
        _context = context;
    }

    [RequirePermission("Finance avancée", "Voir")]
    public IActionResult Index()
    {
        ViewBag.Reconciliations = _context.BankReconciliations.OrderByDescending(r => r.StatementDate).ToList();
        ViewBag.Forecasts = _context.CashflowForecasts.OrderByDescending(f => f.Period).ToList();
        ViewBag.InvoicesTotal = _context.Invoices
            .Select(i => i.TotalAmount)
            .AsEnumerable()
            .DefaultIfEmpty(0m)
            .Sum();

        ViewBag.PaymentsTotal = _context.Payments
            .Select(p => p.Amount)
            .AsEnumerable()
            .DefaultIfEmpty(0m)
            .Sum();

        ViewBag.ExpensesTotal = _context.Expenses
            .Select(e => e.Amount)
            .AsEnumerable()
            .DefaultIfEmpty(0m)
            .Sum();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Finance avancée", "Créer")]
    public IActionResult CreateReconciliation(BankReconciliation reconciliation)
    {
        var difference = reconciliation.StatementBalance - reconciliation.ErpBalance;
        reconciliation.Status = difference == 0 ? "Rapproche" : "Ecart a analyser";
        _context.BankReconciliations.Add(reconciliation);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Finance avancée", "Créer")]
    public IActionResult CreateForecast(CashflowForecast forecast)
    {
        _context.CashflowForecasts.Add(forecast);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
}
