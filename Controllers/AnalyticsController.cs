using EnterpriseERP.Attributes;
using EnterpriseERP.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseERP.Controllers
{
    public class AnalyticsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnalyticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [RequirePermission("Dashboard", "Voir")]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            var today = DateTime.Today;
            var monthStart = new DateTime(today.Year, today.Month, 1);

            var payments = _context.Payments
                .Select(p => new { p.Amount, p.PaymentDate, p.Method })
                .AsEnumerable()
                .ToList();

            var invoices = _context.Invoices
                .Include(i => i.Client)
                .AsEnumerable()
                .ToList();

            var expenses = _context.Expenses
                .Select(e => new { e.Amount, e.ExpenseDate, e.Category })
                .AsEnumerable()
                .ToList();

            ViewBag.TotalRevenue = payments.Sum(p => p.Amount);
            ViewBag.MonthRevenue = payments.Where(p => p.PaymentDate >= monthStart).Sum(p => p.Amount);
            ViewBag.TotalExpenses = expenses.Sum(e => e.Amount);
            ViewBag.MonthExpenses = expenses.Where(e => e.ExpenseDate >= monthStart).Sum(e => e.Amount);
            ViewBag.NetProfit = ViewBag.TotalRevenue - ViewBag.TotalExpenses;
            ViewBag.UnpaidInvoices = invoices.Count(i => i.Status == "Unpaid" || i.Status == "Pending");
            ViewBag.TotalClients = _context.Clients.Count();
            ViewBag.TotalProducts = _context.Products.Count();
            ViewBag.TotalQuotes = _context.Quotes.Count();
            ViewBag.TotalOrders = _context.Orders.Count();

            ViewBag.RevenueByMethod = payments
                .GroupBy(p => string.IsNullOrWhiteSpace(p.Method) ? "Non defini" : p.Method)
                .Select(g => new AnalyticsRow(g.Key, g.Sum(x => x.Amount)))
                .OrderByDescending(x => x.Value)
                .ToList();

            ViewBag.ExpensesByCategory = expenses
                .GroupBy(e => string.IsNullOrWhiteSpace(e.Category) ? "Non defini" : e.Category)
                .Select(g => new AnalyticsRow(g.Key, g.Sum(x => x.Amount)))
                .OrderByDescending(x => x.Value)
                .ToList();

            ViewBag.RecentInvoices = invoices
                .OrderByDescending(i => i.InvoiceDate)
                .Take(8)
                .ToList();

            return View();
        }
    }

    public sealed record AnalyticsRow(string Label, decimal Value);
}
