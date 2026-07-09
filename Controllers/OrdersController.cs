using EnterpriseERP.Attributes;
using EnterpriseERP.Data;
using EnterpriseERP.Models;
using EnterpriseERP.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseERP.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [RequirePermission("Commandes", "Voir")]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            var orders = _context.Orders
                .Include(o => o.Client)
                .Include(o => o.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }

        [HttpGet]
        [RequirePermission("Commandes", "Créer")]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            LoadCreateData();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Commandes", "Créer")]
        public IActionResult Create(Order order)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            var product = _context.Products.FirstOrDefault(p => p.Id == order.ProductId);

            if (order.ClientId <= 0 || product == null || order.Quantity <= 0)
            {
                ViewBag.Error = "Veuillez remplir correctement tous les champs.";
                LoadCreateData();
                return View(order);
            }

            if (product.Quantity < order.Quantity)
            {
                ViewBag.Error = "Stock insuffisant pour ce produit.";
                LoadCreateData();
                return View(order);
            }

            order.UnitPrice = product.SalePrice;
            order.TotalAmount = product.SalePrice * order.Quantity;
            order.OrderDate = DateTime.Now;

            product.Quantity -= order.Quantity;

            _context.Orders.Add(order);
            _context.SaveChanges();

            AuditService.Log(
                _context,
                HttpContext,
                "Commandes",
                "Création",
                $"Commande #{order.Id} - {order.Quantity} x {product.Name}",
                entityId: order.Id.ToString()
            );

            AuditService.Log(
                _context,
                HttpContext,
                "Stock",
                "Sortie",
                $"Sortie stock : {order.Quantity} x {product.Name} suite à la commande #{order.Id}",
                entityId: product.Id.ToString()
            );

            return RedirectToAction(nameof(Index));
        }

        private void LoadCreateData()
        {
            ViewBag.Clients = _context.Clients
                .OrderBy(c => c.FullName)
                .ToList();

            ViewBag.Products = _context.Products
                .OrderBy(p => p.Name)
                .ToList();
        }
    }
}