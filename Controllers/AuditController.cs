using EnterpriseERP.Attributes;
using EnterpriseERP.Data;
using EnterpriseERP.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseERP.Controllers
{
    public class AuditController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuditController(ApplicationDbContext context)
        {
            _context = context;
        }

        [RequirePermission("Audit", "Voir")]
        public IActionResult Index(string? search, string? module, string? severity)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            var logs = _context.AuditLogs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                logs = logs.Where(l =>
                    l.UserName.Contains(search) ||
                    l.UserEmail.Contains(search) ||
                    l.Action.Contains(search) ||
                    l.Description.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(module))
            {
                logs = logs.Where(l => l.Module == module);
            }

            if (!string.IsNullOrWhiteSpace(severity))
            {
                logs = logs.Where(l => l.Severity == severity);
            }

            ViewBag.Search = search;
            ViewBag.Module = module;
            ViewBag.Severity = severity;

            ViewBag.Modules = _context.AuditLogs
                .Select(l => l.Module)
                .Distinct()
                .OrderBy(m => m)
                .ToList();

            ViewBag.Severities = _context.AuditLogs
                .Select(l => l.Severity)
                .Distinct()
                .OrderBy(s => s)
                .ToList();

            ViewBag.TotalLogs = _context.AuditLogs.Count();
            ViewBag.WarningLogs = _context.AuditLogs.Count(l => l.Severity == "Warning");
            ViewBag.ErrorLogs = _context.AuditLogs.Count(l => l.Severity == "Error" || l.Severity == "Critical");
            ViewBag.TodayLogs = _context.AuditLogs.Count(l => l.Date.Date == DateTime.Today);

            var result = logs
                .OrderByDescending(l => l.Date)
                .Take(300)
                .ToList();

            AuditService.Log(
                _context,
                HttpContext,
                "Audit",
                "Consultation",
                "Consultation du centre d'audit"
            );

            return View(result);
        }

        [RequirePermission("Audit", "Voir")]
        public IActionResult Details(int id)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            var log = _context.AuditLogs.FirstOrDefault(l => l.Id == id);

            if (log == null)
                return NotFound();

            return View(log);
        }
    }
}