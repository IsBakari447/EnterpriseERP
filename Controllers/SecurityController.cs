using EnterpriseERP.Attributes;
using EnterpriseERP.Data;
using EnterpriseERP.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseERP.Controllers
{
    public class SecurityController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SecurityController(ApplicationDbContext context)
        {
            _context = context;
        }

        [RequirePermission("Audit", "Voir")]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            ViewBag.TotalUsers = _context.Users.Count();
            ViewBag.ActiveUsers = _context.Users.Count(u => u.IsActive);
            ViewBag.InactiveUsers = _context.Users.Count(u => !u.IsActive);
            ViewBag.PendingUsers = _context.Users.Count(u => !u.IsApproved);
            ViewBag.SuperAdmins = _context.Users.Count(u => u.IsSuperAdmin);

            ViewBag.AccessDeniedLogs = _context.AuditLogs
                .Where(a => a.Action == "Accès refusé")
                .OrderByDescending(a => a.Date)
                .Take(20)
                .ToList();

            ViewBag.LoginLogs = _context.AuditLogs
                .Where(a =>
                    a.Action == "Login" ||
                    a.Action == "Connexion")
                .OrderByDescending(a => a.Date)
                .Take(20)
                .ToList();

            ViewBag.WarningLogs = _context.AuditLogs
                .Where(a =>
                    a.Severity == "Warning" ||
                    a.Severity == "Error" ||
                    a.Severity == "Critical")
                .OrderByDescending(a => a.Date)
                .Take(20)
                .ToList();

            ViewBag.RecentSecurityEvents = _context.AuditLogs
                .Where(a =>
                    a.Module == "Sécurité" ||
                    a.Module == "Connexion" ||
                    a.Severity == "Warning" ||
                    a.Severity == "Error" ||
                    a.Severity == "Critical")
                .OrderByDescending(a => a.Date)
                .Take(50)
                .ToList();

            AuditService.Log(
                _context,
                HttpContext,
                "Security Center",
                "Consultation",
                "Consultation du centre de sécurité"
            );

            return View();
        }
    }
}