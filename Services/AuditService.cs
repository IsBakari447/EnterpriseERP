using EnterpriseERP.Data;
using EnterpriseERP.Models;
using Microsoft.AspNetCore.Http;

namespace EnterpriseERP.Services
{
    public static class AuditService
    {
        public static void Log(
            ApplicationDbContext context,
            HttpContext httpContext,
            string module,
            string action,
            string description,
            string? oldValue = null,
            string? newValue = null,
            string severity = "Information",
            bool success = true,
            string? entityId = null)
        {
            var log = new AuditLog
            {
                Date = DateTime.Now,

                UserName = httpContext.Session.GetString("UserName") ?? "Utilisateur inconnu",
                UserEmail = httpContext.Session.GetString("UserEmail") ?? "",
                UserRole = httpContext.Session.GetString("UserRole") ?? "",

                Module = module,
                Action = action,
                Description = description,

                OldValue = oldValue,
                NewValue = newValue,

                IpAddress = httpContext.Connection.RemoteIpAddress?.ToString(),
                Browser = httpContext.Request.Headers["User-Agent"].ToString(),
                MachineName = Environment.MachineName,

                Success = success,
                Severity = severity,
                EntityId = entityId,
                SessionId = httpContext.Session.Id
            };

            context.AuditLogs.Add(log);
            context.SaveChanges();
        }
    }
}