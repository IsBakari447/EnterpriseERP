using EnterpriseERP.Data;
using EnterpriseERP.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EnterpriseERP.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequirePermissionAttribute : ActionFilterAttribute
    {
        private readonly string _module;
        private readonly string _action;

        public RequirePermissionAttribute(string module, string action)
        {
            _module = module;
            _action = action;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var db = context.HttpContext.RequestServices
                .GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;

            if (db == null)
            {
                context.Result = new StatusCodeResult(500);
                return;
            }

            bool allowed = PermissionService.HasPermission(
                db,
                context.HttpContext,
                _module,
                _action
            );

            if (!allowed)
            {
                AuditService.Log(
                    db,
                    context.HttpContext,
                    "Sécurité",
                    "Accès refusé",
                    $"Tentative d'accès au module '{_module}' - Action '{_action}'",
                    severity: "Warning",
                    success: false
                );

                context.Result = new RedirectToActionResult(
                    "AccessDenied",
                    "Account",
                    null
                );

                return;
            }

            base.OnActionExecuting(context);
        }
    }
}