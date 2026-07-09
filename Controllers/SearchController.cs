using EnterpriseERP.Data;
using EnterpriseERP.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseERP.Controllers
{
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string? q, string? type)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            q = q ?? "";
            type = type ?? "all";

            ViewBag.Query = q;
            ViewBag.Type = type;

            // Initialisation des résultats
            ViewBag.Employees = new List<EnterpriseERP.Models.Employee>();
            ViewBag.Clients = new List<EnterpriseERP.Models.Client>();
            ViewBag.Products = new List<EnterpriseERP.Models.Product>();
            ViewBag.Invoices = new List<EnterpriseERP.Models.Invoice>();
            ViewBag.StockMovements = new List<EnterpriseERP.Models.StockMovement>();
            ViewBag.Presences = new List<EnterpriseERP.Models.Presence>();

            // 🔍 Recherche Clients
            if (type == "all" || type == "clients")
            {
                var clients = _context.Clients.AsQueryable();

                if (!string.IsNullOrWhiteSpace(q))
                {
                    clients = clients.Where(c =>
                        c.FullName.Contains(q) ||
                        c.CompanyName.Contains(q) ||
                        c.Email.Contains(q) ||
                        c.Phone.Contains(q) ||
                        c.Address.Contains(q));
                }

                ViewBag.Clients = clients.ToList();
            }

            // 🔍 Recherche Employés
            if (type == "all" || type == "employees")
            {
                var employees = _context.Employees.AsQueryable();

                if (!string.IsNullOrWhiteSpace(q))
                {
                    employees = employees.Where(e =>
                        e.FullName.Contains(q) ||
                        e.Position.Contains(q) ||
                        e.Email.Contains(q) ||
                        e.Phone.Contains(q));
                }

                ViewBag.Employees = employees.ToList();
            }

            // 🔍 Recherche Produits
            if (type == "all" || type == "products")
            {
                var products = _context.Products.AsQueryable();

                if (!string.IsNullOrWhiteSpace(q))
                {
                    products = products.Where(p =>
                        p.Name.Contains(q) ||
                        p.Category.Contains(q));
                }

                ViewBag.Products = products.ToList();
            }

            // 🔍 Recherche Factures
            if (type == "all" || type == "invoices")
            {
                var invoices = _context.Invoices
                    .Include(i => i.Client)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(q))
                {
                    invoices = invoices.Where(i =>
                        i.Status.Contains(q) ||
                        i.Id.ToString().Contains(q));
                }

                ViewBag.Invoices = invoices.ToList();
            }

            // 🔍 Recherche Stock
            if (type == "all" || type == "stock")
            {
                var stock = _context.StockMovements
                    .Include(s => s.Product)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(q))
                {
                    stock = stock.Where(s =>
                        s.Type.Contains(q) ||
                        s.Product!.Name.Contains(q));
                }

                ViewBag.StockMovements = stock.ToList();
            }

            // 🔍 Recherche Présences
            if (type == "all" || type == "presence")
            {
                var presences = _context.Presences
                    .Include(p => p.Employee)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(q))
                {
                    presences = presences.Where(p =>
                        p.Employee!.FullName.Contains(q));
                }

                ViewBag.Presences = presences.ToList();
            }

            //  Audit de recherche
            AuditService.Log(
                _context,
                HttpContext,
                "Recherche",
                "Recherche globale",
                $"Recherche effectuée : '{q}' dans '{type}'"
            );

            return View();
        }
    }
}