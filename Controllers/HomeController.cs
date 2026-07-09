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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
