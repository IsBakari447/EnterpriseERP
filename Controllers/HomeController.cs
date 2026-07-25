using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EnterpriseERP.Models;

namespace EnterpriseERP.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Features() => View();
    public IActionResult Downloads() => View();
    public IActionResult Pricing() => View();
    public IActionResult Contact() => View();
    public IActionResult Demo() => View();
    public IActionResult Manual()
    {
        var lang = Services.TranslationService.NormalizeLanguage(
            HttpContext.Session.GetString("Language")
            ?? Request.Cookies["Language"]);
        var manualFile = lang == "fr" ? "MANUEL_UTILISATEUR.md" : $"MANUEL_UTILISATEUR.{lang}.md";
        var manualPath = Path.Combine(Directory.GetCurrentDirectory(), manualFile);
        if (!System.IO.File.Exists(manualPath))
            manualPath = Path.Combine(Directory.GetCurrentDirectory(), "MANUEL_UTILISATEUR.md");

        ViewBag.ManualMarkdown = System.IO.File.Exists(manualPath)
            ? System.IO.File.ReadAllText(manualPath)
            : "# Manuel d'utilisation\n\nLe manuel n'est pas encore disponible.";
        ViewBag.ManualLanguage = lang;

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
