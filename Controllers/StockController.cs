using EnterpriseERP.Attributes;
using EnterpriseERP.Data;
using EnterpriseERP.Models;
using EnterpriseERP.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseERP.Controllers
{
    public class StockController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StockController(ApplicationDbContext context)
        {
            _context = context;
        }

        [RequirePermission("Stock", "Voir")]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            var movements = _context.StockMovements
                .Include(s => s.Product)
                .OrderByDescending(s => s.Date)
                .ToList();

            return View(movements);
        }

        [HttpGet]
        [RequirePermission("Stock", "Créer")]
        public IActionResult Movement()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            LoadProducts();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Stock", "Créer")]
        public IActionResult Movement(StockMovement movement)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            var product = _context.Products.FirstOrDefault(p => p.Id == movement.ProductId);

            if (product == null)
            {
                ViewBag.Error = "Produit introuvable.";
                LoadProducts();
                return View(movement);
            }

            if (movement.Quantity <= 0)
            {
                ViewBag.Error = "La quantité doit être supérieure à zéro.";
                LoadProducts();
                return View(movement);
            }

            int oldQuantity = product.Quantity;

            movement.Date = DateTime.Now;

            if (movement.Type == "IN")
            {
                product.Quantity += movement.Quantity;
            }
            else if (movement.Type == "OUT")
            {
                if (product.Quantity < movement.Quantity)
                {
                    ViewBag.Error = "Stock insuffisant.";
                    LoadProducts();
                    return View(movement);
                }

                product.Quantity -= movement.Quantity;
            }
            else
            {
                ViewBag.Error = "Type de mouvement invalide.";
                LoadProducts();
                return View(movement);
            }

            _context.StockMovements.Add(movement);
            _context.SaveChanges();

            AuditService.Log(
                _context,
                HttpContext,
                "Stock",
                "Mouvement",
                $"Stock modifié pour {product.Name} ({movement.Type} : {movement.Quantity})",
                oldValue: $"Ancien stock : {oldQuantity}",
                newValue: $"Nouveau stock : {product.Quantity}",
                entityId: product.Id.ToString()
            );

            return RedirectToAction(nameof(Index));
        }

        private void LoadProducts()
        {
            ViewBag.Products = _context.Products
                .OrderBy(p => p.Name)
                .ToList();
        }
    }
}