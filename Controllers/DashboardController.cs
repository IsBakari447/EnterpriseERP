using EnterpriseERP.Attributes;
using EnterpriseERP.Data;
using EnterpriseERP.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseERP.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly TranslationService _translator;

        public DashboardController(ApplicationDbContext context, TranslationService translator)
        {
            _context = context;
            _translator = translator;
        }

        [RequirePermission("Dashboard", "Voir")]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");

            var today = DateTime.Today;
            var monthStart = new DateTime(today.Year, today.Month, 1);
            var yearStart = new DateTime(today.Year, 1, 1);

            int lowStockThreshold = _context.AppSettings
                .Select(s => s.LowStockThreshold)
                .FirstOrDefault();

            if (lowStockThreshold <= 0)
                lowStockThreshold = 5;

            // Base
            ViewBag.TotalEmployees = _context.Employees.Count();
            ViewBag.TotalClients = _context.Clients.Count();
            ViewBag.TotalProducts = _context.Products.Count();
            ViewBag.TotalSuppliers = _context.Suppliers.Count();
            ViewBag.TotalStock = _context.Products.Sum(p => (int?)p.Quantity) ?? 0;

            // Devis
            ViewBag.TotalQuotes = _context.Quotes.Count();
            ViewBag.TotalClients = _context.Clients.Count();
            ViewBag.AcceptedQuotes = _context.Quotes.Count(q => q.Status == "Accepté");
            ViewBag.PendingQuotes = _context.Quotes.Count(q => q.Status == "Brouillon" || q.Status == "Envoyé");
            ViewBag.TotalQuoteAmount = _context.Quotes.Sum(q => (decimal?)q.TotalAmount) ?? 0;

            // Factures
            ViewBag.TotalInvoices = _context.Invoices.Count();
            ViewBag.PaidInvoices = _context.Invoices.Count(i => i.Status == "Paid");
            ViewBag.UnpaidInvoices = _context.Invoices.Count(i => i.Status == "Unpaid" || i.Status == "Pending");
            ViewBag.TotalUnpaidAmount = _context.Invoices
                .Where(i => i.Status == "Unpaid" || i.Status == "Pending")
                .Sum(i => (decimal?)i.TotalAmount) ?? 0;

            // Commandes / Paiements / Dépenses
            ViewBag.TotalOrders = _context.Orders.Count();
            ViewBag.PendingOrdersCount = _context.Orders.Count(o => o.Status == "Pending");
            ViewBag.TotalPayments = _context.Payments.Count();
            ViewBag.TotalExpenses = _context.Expenses.Sum(e => (decimal?)e.Amount) ?? 0;
            ViewBag.MonthExpenses = _context.Expenses
                .Where(e => e.ExpenseDate >= monthStart)
                .Sum(e => (decimal?)e.Amount) ?? 0;

            // Revenus
            ViewBag.TotalRevenue = _context.Payments.Sum(p => (decimal?)p.Amount) ?? 0;
            ViewBag.MonthRevenue = _context.Payments
                .Where(p => p.PaymentDate >= monthStart)
                .Sum(p => (decimal?)p.Amount) ?? 0;
            ViewBag.YearRevenue = _context.Payments
                .Where(p => p.PaymentDate >= yearStart)
                .Sum(p => (decimal?)p.Amount) ?? 0;
            ViewBag.PaymentsToday = _context.Payments
                .Where(p => p.PaymentDate.Date == today)
                .Sum(p => (decimal?)p.Amount) ?? 0;

            ViewBag.NetProfit = ViewBag.TotalRevenue - ViewBag.TotalExpenses;
            ViewBag.MonthNetProfit = ViewBag.MonthRevenue - ViewBag.MonthExpenses;

            // Alertes
            ViewBag.LowStock = _context.Products.Count(p => p.Quantity <= lowStockThreshold);
            ViewBag.TodayPresences = _context.Presences.Count(p => p.Date == today);

            ViewBag.LowStockProducts = _context.Products
                .Where(p => p.Quantity <= lowStockThreshold)
                .OrderBy(p => p.Quantity)
                .Take(10)
                .ToList();

            // Listes récentes
            ViewBag.LatestQuotes = _context.Quotes
                .Include(q => q.Client)
                .OrderByDescending(q => q.QuoteDate)
                .Take(10)
                .ToList();

            ViewBag.LatestInvoices = _context.Invoices
                .Include(i => i.Client)
                .OrderByDescending(i => i.InvoiceDate)
                .Take(10)
                .ToList();

            ViewBag.UnpaidInvoicesList = _context.Invoices
                .Include(i => i.Client)
                .Where(i => i.Status == "Unpaid" || i.Status == "Pending")
                .OrderByDescending(i => i.InvoiceDate)
                .Take(10)
                .ToList();

            ViewBag.PendingOrders = _context.Orders
                .Include(o => o.Client)
                .Include(o => o.Product)
                .Where(o => o.Status == "Pending")
                .OrderByDescending(o => o.OrderDate)
                .Take(10)
                .ToList();

            ViewBag.RecentPayments = _context.Payments
                .Include(p => p.Invoice)
                .ThenInclude(i => i!.Client)
                .OrderByDescending(p => p.PaymentDate)
                .Take(10)
                .ToList();

            ViewBag.LatestEmployees = _context.Employees
                .OrderByDescending(e => e.CreatedAt)
                .Take(5)
                .ToList();

            ViewBag.LatestClients = _context.Clients
                .OrderByDescending(c => c.CreatedAt)
                .Take(5)
                .ToList();

            // Graphiques
            var monthlyRevenue = _context.Payments
                .GroupBy(p => new { p.PaymentDate.Year, p.PaymentDate.Month })
                .Select(g => new
                {
                    Label = g.Key.Month + "/" + g.Key.Year,
                    Total = g.Sum(p => p.Amount)
                })
                .ToList();

            ViewBag.MonthLabels = monthlyRevenue.Select(x => x.Label).ToList();
            ViewBag.MonthRevenues = monthlyRevenue.Select(x => x.Total).ToList();

            var monthlyExpenses = _context.Expenses
                .GroupBy(e => new { e.ExpenseDate.Year, e.ExpenseDate.Month })
                .Select(g => new
                {
                    Label = g.Key.Month + "/" + g.Key.Year,
                    Total = g.Sum(e => e.Amount)
                })
                .ToList();

            ViewBag.ExpenseMonthLabels = monthlyExpenses.Select(x => x.Label).ToList();
            ViewBag.ExpenseMonthTotals = monthlyExpenses.Select(x => x.Total).ToList();

            var monthlyOrders = _context.Orders
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new
                {
                    Label = g.Key.Month + "/" + g.Key.Year,
                    Total = g.Count()
                })
                .ToList();

            ViewBag.OrderMonthLabels = monthlyOrders.Select(x => x.Label).ToList();
            ViewBag.OrderMonthCounts = monthlyOrders.Select(x => x.Total).ToList();

            var paymentsByMethod = _context.Payments
                .GroupBy(p => p.Method)
                .Select(g => new
                {
                    Method = g.Key,
                    Total = g.Sum(p => p.Amount)
                })
                .ToList();

            ViewBag.PaymentMethods = paymentsByMethod.Select(x => x.Method).ToList();
            ViewBag.PaymentMethodTotals = paymentsByMethod.Select(x => x.Total).ToList();

            ViewBag.ProductNames = _context.Products
                .OrderBy(p => p.Name)
                .Select(p => p.Name)
                .ToList();

            ViewBag.ProductQuantities = _context.Products
                .OrderBy(p => p.Name)
                .Select(p => p.Quantity)
                .ToList();

            var topProducts = _context.Orders
                .Include(o => o.Product)
                .Where(o => o.Product != null)
                .GroupBy(o => o.Product!.Name)
                .Select(g => new
                {
                    Product = g.Key,
                    Quantity = g.Sum(o => o.Quantity)
                })
                .OrderByDescending(x => x.Quantity)
                .Take(5)
                .ToList();

            ViewBag.TopProductNames = topProducts.Select(x => x.Product).ToList();
            ViewBag.TopProductQuantities = topProducts.Select(x => x.Quantity).ToList();

            AuditService.Log(
                _context,
                HttpContext,
                "Dashboard",
                "Consultation",
                "Consultation du dashboard Enterprise 2.0"
            );

            ViewBag.CurrentDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            ViewBag.CurrentLanguage =
                HttpContext.Session.GetString("Language")
                ?? _context.AppSettings.Select(s => s.DefaultLanguage).FirstOrDefault()
                ?? "fr";

            ViewBag.TitleTranslated = _translator.T("Dashboard");

            return View();
        }
    }
}