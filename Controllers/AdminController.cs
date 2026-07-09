using EnterpriseERP.Attributes;
using EnterpriseERP.Data;
using EnterpriseERP.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseERP.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [RequirePermission("Paramètres", "Voir")]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            bool isSuperAdmin = HttpContext.Session.GetString("IsSuperAdmin") == "true";
            string role = HttpContext.Session.GetString("UserRole") ?? "";

            if (!isSuperAdmin && role != "SuperAdmin")
                return PermissionService.Deny();

            ViewBag.UsersCount = _context.Users.Count();
            ViewBag.ActiveUsers = _context.Users.Count(u => u.IsActive);
            ViewBag.PendingUsers = _context.Users.Count(u => !u.IsApproved || !u.IsActive);
            ViewBag.RolesCount = _context.RolePermissions
                .Select(r => r.Role)
                .Distinct()
                .Count();

            ViewBag.AuditLogsCount = _context.AuditLogs.Count();
            ViewBag.SecurityWarnings = _context.AuditLogs.Count(a =>
                a.Severity == "Warning" ||
                a.Severity == "Error" ||
                a.Severity == "Critical");

            ViewBag.PermissionsCount = _context.Permissions.Count();
            ViewBag.SettingsCount = _context.AppSettings.Count();

            AuditService.Log(
                _context,
                HttpContext,
                "Admin Center",
                "Consultation",
                "Consultation du centre d'administration"
            );

            return View();
        }
    }
}