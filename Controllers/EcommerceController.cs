using EnterpriseERP.Attributes;
using EnterpriseERP.Data;
using EnterpriseERP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseERP.Controllers;

public class EcommerceController : Controller
{
    private readonly ApplicationDbContext _context;

    public EcommerceController(ApplicationDbContext context)
    {
        _context = context;
    }

    [RequirePermission("Ecommerce", "Voir")]
    public IActionResult Index()
    {
        ViewBag.Connections = _context.EcommerceConnections.OrderByDescending(c => c.CreatedAt).ToList();
        ViewBag.Products = _context.Products.OrderBy(p => p.Category).ThenBy(p => p.Name).ToList();
        ViewBag.OpenOrders = _context.Orders.Include(o => o.Client).Include(o => o.Product).OrderByDescending(o => o.OrderDate).Take(20).ToList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Ecommerce", "Créer")]
    public IActionResult Connect(EcommerceConnection connection)
    {
        connection.SyncStatus = "Connecteur cree";
        _context.EcommerceConnections.Add(connection);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Ecommerce", "Modifier")]
    public IActionResult MarkSynced(int id)
    {
        var connection = _context.EcommerceConnections.Find(id);
        if (connection == null)
            return NotFound();

        connection.SyncStatus = "Synchronise";
        connection.LastSyncAt = DateTime.Now;
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
}
