using EnterpriseERP.Attributes;
using EnterpriseERP.Data;
using EnterpriseERP.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseERP.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [RequirePermission("Notifications", "Voir")]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            int lowStockThreshold = _context.AppSettings
                .Select(s => s.LowStockThreshold)
                .FirstOrDefault();

            if (lowStockThreshold <= 0)
                lowStockThreshold = 5;

            ViewBag.OutOfStockProducts = _context.Products
                .Where(p => p.Quantity <= 0)
                .OrderBy(p => p.Name)
                .ToList();

            ViewBag.LowStockProducts = _context.Products
                .Where(p => p.Quantity > 0 && p.Quantity <= lowStockThreshold)
                .OrderBy(p => p.Quantity)
                .ToList();

            ViewBag.UnpaidInvoices = _context.Invoices
                .Include(i => i.Client)
                .Where(i => i.Status == "Unpaid" || i.Status == "Pending")
                .OrderByDescending(i => i.InvoiceDate)
                .ToList();

            ViewBag.PendingOrders = _context.Orders
                .Include(o => o.Client)
                .Include(o => o.Product)
                .Where(o => o.Status == "Pending")
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            ViewBag.RecentPayments = _context.Payments
                .Include(p => p.Invoice)
                .ThenInclude(i => i!.Client)
                .OrderByDescending(p => p.PaymentDate)
                .Take(10)
                .ToList();

            ViewBag.PendingUsers = _context.Users
                .Where(u => !u.IsApproved || !u.IsActive)
                .OrderByDescending(u => u.CreatedAt)
                .ToList();

            ViewBag.TodayPresences = _context.Presences
                .Include(p => p.Employee)
                .Where(p => p.Date.Date == DateTime.Today)
                .OrderBy(p => p.Employee!.FullName)
                .ToList();

            ViewBag.TotalAlerts =
                ((List<EnterpriseERP.Models.Product>)ViewBag.OutOfStockProducts).Count +
                ((List<EnterpriseERP.Models.Product>)ViewBag.LowStockProducts).Count +
                ((List<EnterpriseERP.Models.Invoice>)ViewBag.UnpaidInvoices).Count +
                ((List<EnterpriseERP.Models.Order>)ViewBag.PendingOrders).Count +
                ((List<EnterpriseERP.Models.User>)ViewBag.PendingUsers).Count;

            AuditService.Log(
                _context,
                HttpContext,
                "Notifications",
                "Consultation",
                "Consultation du tableau des notifications"
            );

            return View();
        }
    }
}