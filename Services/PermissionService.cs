using EnterpriseERP.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseERP.Services
{
    public static class PermissionService
    {
        public static bool HasPermission(
            ApplicationDbContext context,
            HttpContext httpContext,
            string module,
            string action)
        {
            string role = httpContext.Session.GetString("UserRole") ?? "";
            bool isSuperAdmin = httpContext.Session.GetString("IsSuperAdmin") == "true";

            if (isSuperAdmin || role == "SuperAdmin")
                return true;

            return context.RolePermissions
                .Include(rp => rp.Permission)
                .Any(rp =>
                    rp.Role == role &&
                    rp.Permission != null &&
                    rp.Permission.Module == module &&
                    rp.Permission.Action == action
                );
        }

        public static IActionResult Deny()
        {
            return new RedirectToActionResult("AccessDenied", "Account", null);
        }
    }
}