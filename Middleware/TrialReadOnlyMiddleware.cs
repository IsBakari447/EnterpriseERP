using EnterpriseERP.Services.Trial;

namespace EnterpriseERP.Middleware;

public sealed class TrialReadOnlyMiddleware
{
    private static readonly HashSet<string> AllowedWritePrefixes = new(StringComparer.OrdinalIgnoreCase)
    {
        "/Account/Login",
        "/Account/Logout",
        "/Language/Set",
        "/api/mobile/auth"
    };

    private readonly RequestDelegate _next;

    public TrialReadOnlyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, TrialPolicyService trialPolicy)
    {
        if (!IsWriteRequest(context) || IsAllowedWritePath(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var status = await trialPolicy.GetStatusAsync(context.RequestAborted);
        if (!status.IsReadOnly)
        {
            await _next(context);
            return;
        }

        context.Response.StatusCode = StatusCodes.Status402PaymentRequired;

        if (context.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
        {
            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "Essai gratuit termine : lecture seule. Paiement obligatoire pour continuer.",
                trial = status
            }, context.RequestAborted);
            return;
        }

        context.Response.ContentType = "text/html; charset=utf-8";
        await context.Response.WriteAsync(
            "<h1>Essai gratuit termine</h1><p>Votre espace EnterpriseERP est maintenant en lecture seule. Paiement obligatoire pour continuer. Les donnees sont conservees 90 jours.</p>",
            context.RequestAborted);
    }

    private static bool IsWriteRequest(HttpContext context)
    {
        return HttpMethods.IsPost(context.Request.Method)
            || HttpMethods.IsPut(context.Request.Method)
            || HttpMethods.IsPatch(context.Request.Method)
            || HttpMethods.IsDelete(context.Request.Method);
    }

    private static bool IsAllowedWritePath(PathString path)
    {
        return AllowedWritePrefixes.Any(prefix => path.StartsWithSegments(prefix, StringComparison.OrdinalIgnoreCase));
    }
}
