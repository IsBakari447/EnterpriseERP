using Microsoft.AspNetCore.Mvc;

namespace EnterpriseERP.Controllers;

public class LanguageController : Controller
{
    [HttpGet]
    public IActionResult Set(string lang, string? returnUrl = null)
    {
        lang = Services.TranslationService.NormalizeLanguage(lang);

        HttpContext.Session.SetString("Language", lang);

        Response.Cookies.Append("Language", lang, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddYears(1),
            IsEssential = true,
            Path = "/",
            SameSite = SameSiteMode.Lax,
            HttpOnly = false
        });

        if (string.IsNullOrWhiteSpace(returnUrl) || !Url.IsLocalUrl(returnUrl))
            returnUrl = "/";

        return LocalRedirect(returnUrl);
    }
}
