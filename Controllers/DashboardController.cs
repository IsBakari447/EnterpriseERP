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
        public async Task<IActionResult> Index()
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var monthStart = new DateTime(today.Year, today.Month, 1);
            var yearStart = new DateTime(today.Year, 1, 1);

            var lowStockThreshold = await _context.AppSettings
                .AsNoTracking()
                .Select(s => s.LowStockThreshold)
                .FirstOrDefaultAsync();

            if (lowStockThreshold <= 0)
                lowStockThreshold = 5;

            ViewBag.TotalEmployees = await _context.Employees.AsNoTracking().CountAsync();
            ViewBag.TotalClients = await _context.Clients.AsNoTracking().CountAsync();
            ViewBag.TotalProducts = await _context.Products.AsNoTracking().CountAsync();
            ViewBag.TotalSuppliers = await _context.Suppliers.AsNoTracking().CountAsync();
            ViewBag.TotalStock = await _context.Products.AsNoTracking().SumAsync(p => (int?)p.Quantity) ?? 0;

            ViewBag.TotalQuotes = await _context.Quotes.AsNoTracking().CountAsync();
            ViewBag.AcceptedQuotes = await _context.Quotes.AsNoTracking().CountAsync(q => q.Status == "Accepte" || q.Status == "Accepté");
            ViewBag.PendingQuotes = await _context.Quotes.AsNoTracking().CountAsync(q => q.Status == "Brouillon" || q.Status == "Envoye" || q.Status == "Envoyé");
            ViewBag.TotalQuoteAmount = await _context.Quotes.AsNoTracking().SumAsync(q => (decimal?)q.TotalAmount) ?? 0m;

            ViewBag.TotalInvoices = await _context.Invoices.AsNoTracking().CountAsync();
            ViewBag.PaidInvoices = await _context.Invoices.AsNoTracking().CountAsync(i => i.Status == "Paid");
            ViewBag.UnpaidInvoices = await _context.Invoices.AsNoTracking().CountAsync(i => i.Status == "Unpaid" || i.Status == "Pending");
            ViewBag.TotalUnpaidAmount = await _context.Invoices
                .AsNoTracking()
                .Where(i => i.Status == "Unpaid" || i.Status == "Pending")
                .SumAsync(i => (decimal?)i.TotalAmount) ?? 0m;

            ViewBag.TotalOrders = await _context.Orders.AsNoTracking().CountAsync();
            ViewBag.PendingOrdersCount = await _context.Orders.AsNoTracking().CountAsync(o => o.Status == "Pending");
            ViewBag.TotalPayments = await _context.Payments.AsNoTracking().CountAsync();
            ViewBag.TotalExpenses = await _context.Expenses.AsNoTracking().SumAsync(e => (decimal?)e.Amount) ?? 0m;
            ViewBag.MonthExpenses = await _context.Expenses
                .AsNoTracking()
                .Where(e => e.ExpenseDate >= monthStart)
                .SumAsync(e => (decimal?)e.Amount) ?? 0m;

            ViewBag.TotalRevenue = await _context.Payments.AsNoTracking().SumAsync(p => (decimal?)p.Amount) ?? 0m;
            ViewBag.MonthRevenue = await _context.Payments
                .AsNoTracking()
                .Where(p => p.PaymentDate >= monthStart)
                .SumAsync(p => (decimal?)p.Amount) ?? 0m;
            ViewBag.YearRevenue = await _context.Payments
                .AsNoTracking()
                .Where(p => p.PaymentDate >= yearStart)
                .SumAsync(p => (decimal?)p.Amount) ?? 0m;
            ViewBag.PaymentsToday = await _context.Payments
                .AsNoTracking()
                .Where(p => p.PaymentDate >= today && p.PaymentDate < tomorrow)
                .SumAsync(p => (decimal?)p.Amount) ?? 0m;

            ViewBag.NetProfit = ViewBag.TotalRevenue - ViewBag.TotalExpenses;
            ViewBag.MonthNetProfit = ViewBag.MonthRevenue - ViewBag.MonthExpenses;
            ViewBag.LowStock = await _context.Products.AsNoTracking().CountAsync(p => p.Quantity <= lowStockThreshold);
            ViewBag.TodayPresences = await _context.Presences.AsNoTracking().CountAsync(p => p.Date == today);

            ViewBag.LowStockProducts = await _context.Products
                .AsNoTracking()
                .Where(p => p.Quantity <= lowStockThreshold)
                .OrderBy(p => p.Quantity)
                .Take(10)
                .ToListAsync();

            ViewBag.LatestQuotes = await _context.Quotes
                .AsNoTracking()
                .Include(q => q.Client)
                .OrderByDescending(q => q.QuoteDate)
                .Take(10)
                .ToListAsync();

            ViewBag.LatestInvoices = await _context.Invoices
                .AsNoTracking()
                .Include(i => i.Client)
                .OrderByDescending(i => i.InvoiceDate)
                .Take(10)
                .ToListAsync();

            ViewBag.UnpaidInvoicesList = await _context.Invoices
                .AsNoTracking()
                .Include(i => i.Client)
                .Where(i => i.Status == "Unpaid" || i.Status == "Pending")
                .OrderByDescending(i => i.InvoiceDate)
                .Take(10)
                .ToListAsync();

            ViewBag.PendingOrders = await _context.Orders
                .AsNoTracking()
                .Include(o => o.Client)
                .Include(o => o.Product)
                .Where(o => o.Status == "Pending")
                .OrderByDescending(o => o.OrderDate)
                .Take(10)
                .ToListAsync();

            ViewBag.RecentPayments = await _context.Payments
                .AsNoTracking()
                .Include(p => p.Invoice)
                .ThenInclude(i => i!.Client)
                .OrderByDescending(p => p.PaymentDate)
                .Take(10)
                .ToListAsync();

            ViewBag.LatestEmployees = await _context.Employees
                .AsNoTracking()
                .OrderByDescending(e => e.CreatedAt)
                .Take(5)
                .ToListAsync();

            ViewBag.LatestClients = await _context.Clients
                .AsNoTracking()
                .OrderByDescending(c => c.CreatedAt)
                .Take(5)
                .ToListAsync();

            var paymentRows = await _context.Payments
                .AsNoTracking()
                .Select(p => new { p.PaymentDate, p.Amount, p.Method })
                .ToListAsync();

            var expenseRows = await _context.Expenses
                .AsNoTracking()
                .Select(e => new { e.ExpenseDate, e.Amount })
                .ToListAsync();

            var orderRows = await _context.Orders
                .AsNoTracking()
                .Select(o => new { o.OrderDate })
                .ToListAsync();

            var monthlyRevenue = paymentRows
                .GroupBy(p => new { p.PaymentDate.Year, p.PaymentDate.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Label = $"{g.Key.Month}/{g.Key.Year}",
                    Total = g.Sum(p => p.Amount)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            ViewBag.MonthLabels = monthlyRevenue.Select(x => x.Label).ToList();
            ViewBag.MonthRevenues = monthlyRevenue.Select(x => x.Total).ToList();

            var monthlyExpenses = expenseRows
                .GroupBy(e => new { e.ExpenseDate.Year, e.ExpenseDate.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Label = $"{g.Key.Month}/{g.Key.Year}",
                    Total = g.Sum(e => e.Amount)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            ViewBag.ExpenseMonthLabels = monthlyExpenses.Select(x => x.Label).ToList();
            ViewBag.ExpenseMonthTotals = monthlyExpenses.Select(x => x.Total).ToList();

            var monthlyOrders = orderRows
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Label = $"{g.Key.Month}/{g.Key.Year}",
                    Total = g.Count()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            ViewBag.OrderMonthLabels = monthlyOrders.Select(x => x.Label).ToList();
            ViewBag.OrderMonthCounts = monthlyOrders.Select(x => x.Total).ToList();

            var paymentsByMethod = paymentRows
                .GroupBy(p => string.IsNullOrWhiteSpace(p.Method) ? "Autre" : p.Method)
                .Select(g => new
                {
                    Method = g.Key,
                    Total = g.Sum(p => p.Amount)
                })
                .ToList();

            ViewBag.PaymentMethods = paymentsByMethod.Select(x => x.Method).ToList();
            ViewBag.PaymentMethodTotals = paymentsByMethod.Select(x => x.Total).ToList();

            ViewBag.ProductNames = await _context.Products
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .Select(p => p.Name)
                .ToListAsync();

            ViewBag.ProductQuantities = await _context.Products
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .Select(p => p.Quantity)
                .ToListAsync();

            var topProducts = await _context.Orders
                .AsNoTracking()
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
                .ToListAsync();

            ViewBag.TopProductNames = topProducts.Select(x => x.Product).ToList();
            ViewBag.TopProductQuantities = topProducts.Select(x => x.Quantity).ToList();
            ViewBag.CurrentDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            ViewBag.CurrentLanguage =
                HttpContext.Session.GetString("Language")
                ?? await _context.AppSettings.AsNoTracking().Select(s => s.DefaultLanguage).FirstOrDefaultAsync()
                ?? "fr";
            ViewBag.TitleTranslated = _translator.T("Dashboard");

            return View();
        }
    }
}
