using EnterpriseERP.Attributes;
using EnterpriseERP.Data;
using EnterpriseERP.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseERP.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [RequirePermission("Utilisateurs", "Voir")]
        public IActionResult Index(string? search, string? role, string? status)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            var users = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                users = users.Where(u =>
                    u.FullName.Contains(search) ||
                    u.Email.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(role))
                users = users.Where(u => u.Role == role);

            if (status == "active")
                users = users.Where(u => u.IsActive);

            if (status == "inactive")
                users = users.Where(u => !u.IsActive);

            if (status == "pending")
                users = users.Where(u => !u.IsApproved);

            ViewBag.Search = search;
            ViewBag.Role = role;
            ViewBag.Status = status;

            ViewBag.TotalUsers = _context.Users.Count();
            ViewBag.ActiveUsers = _context.Users.Count(u => u.IsActive);
            ViewBag.InactiveUsers = _context.Users.Count(u => !u.IsActive);
            ViewBag.PendingUsers = _context.Users.Count(u => !u.IsApproved);

            var result = users
                .OrderByDescending(u => u.CreatedAt)
                .ToList();

            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Utilisateurs", "Modifier")]
        public IActionResult ToggleActive(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
                return NotFound();

            if (user.IsSuperAdmin)
            {
                TempData["Error"] = "Impossible de désactiver le SuperAdmin.";
                return RedirectToAction(nameof(Index));
            }

            bool oldStatus = user.IsActive;
            user.IsActive = !user.IsActive;
            user.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            AuditService.Log(
                _context,
                HttpContext,
                "Utilisateurs",
                "Activation/Désactivation",
                $"Compte {(user.IsActive ? "activé" : "désactivé")} : {user.FullName}",
                oldValue: oldStatus ? "Actif" : "Inactif",
                newValue: user.IsActive ? "Actif" : "Inactif",
                entityId: user.Id.ToString(),
                severity: "Warning"
            );

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Utilisateurs", "Modifier")]
        public IActionResult Approve(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
                return NotFound();

            user.IsApproved = true;
            user.IsActive = true;
            user.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            AuditService.Log(
                _context,
                HttpContext,
                "Utilisateurs",
                "Approbation",
                $"Compte approuvé : {user.FullName}",
                entityId: user.Id.ToString()
            );

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Utilisateurs", "Modifier")]
        public IActionResult ChangeRole(int id, string role)
        {
            bool isSuperAdminSession = HttpContext.Session.GetString("IsSuperAdmin") == "true";

            if (!isSuperAdminSession)
            {
                TempData["Error"] = "Seul le SuperAdmin peut changer les rôles.";
                return RedirectToAction(nameof(Index));
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
                return NotFound();

            if (user.IsSuperAdmin)
            {
                TempData["Error"] = "Impossible de modifier le rôle du SuperAdmin.";
                return RedirectToAction(nameof(Index));
            }

            if (role == "SuperAdmin")
            {
                TempData["Error"] = "Impossible de créer un second SuperAdmin ici.";
                return RedirectToAction(nameof(Index));
            }

            string oldRole = user.Role;
            user.Role = role;
            user.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            AuditService.Log(
                _context,
                HttpContext,
                "Utilisateurs",
                "Changement rôle",
                $"Rôle modifié pour {user.FullName}",
                oldValue: oldRole,
                newValue: role,
                entityId: user.Id.ToString(),
                severity: "Warning"
            );

            return RedirectToAction(nameof(Index));
        }
    }
}