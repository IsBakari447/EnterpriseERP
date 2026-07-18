using EnterpriseERP.Data;
using EnterpriseERP.Models;
using EnterpriseERP.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseERP.Controllers
{
    public class RolesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RolesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("IsSuperAdmin") != "true")
                return PermissionService.Deny();

            SeedPermissions();

            ViewBag.Roles = new List<string>
            {
                "Admin",
                "Manager",
                "Employee"
            };

            ViewBag.Permissions = _context.Permissions
                .OrderBy(p => p.Module)
                .ThenBy(p => p.Action)
                .ToList();

            ViewBag.RolePermissions = _context.RolePermissions
                .Include(rp => rp.Permission)
                .ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(string role, List<int> permissionIds)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("IsSuperAdmin") != "true")
                return PermissionService.Deny();

            var existing = _context.RolePermissions
                .Where(rp => rp.Role == role)
                .ToList();

            _context.RolePermissions.RemoveRange(existing);

            foreach (var permissionId in permissionIds)
            {
                _context.RolePermissions.Add(new RolePermission
                {
                    Role = role,
                    PermissionId = permissionId
                });
            }

            _context.SaveChanges();

            AuditService.Log(
                _context,
                HttpContext,
                "Rôles",
                "Modification permissions",
                $"Permissions mises à jour pour le rôle : {role}",
                severity: "Warning"
            );

            return RedirectToAction(nameof(Index));
        }

        private void SeedPermissions()
        {
            var modules = new List<string>
            {
                "Dashboard",
                "Clients",
                "Employés",
                "Produits",
                "Stock",
                "Factures",
                "Commandes",
                "Fournisseurs",
                "Paiements",
                "Présences",
                "Exports",
                "Devis",
                "Notifications",
                "Social",
                "Paramètres",
                "Audit",
                "Utilisateurs"
            };

            var actions = new List<string>
            {
                "Voir",
                "Créer",
                "Modifier",
                "Supprimer",
                "Exporter"
            };

            foreach (var module in modules)
            {
                foreach (var action in actions)
                {
                    bool exists = _context.Permissions.Any(p =>
                        p.Module == module &&
                        p.Action == action
                    );

                    if (!exists)
                    {
                        _context.Permissions.Add(new Permission
                        {
                            Module = module,
                            Action = action,
                            Description = $"{action} {module}"
                        });
                    }
                }
            }

            _context.SaveChanges();
        }
    }
}
