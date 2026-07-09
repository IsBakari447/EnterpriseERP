using EnterpriseERP.Attributes;
using EnterpriseERP.Data;
using EnterpriseERP.Models;
using EnterpriseERP.Services;
using EnterpriseERP.Services.Trial;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseERP.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly TrialPolicyService _trialPolicy;

        public ProductsController(ApplicationDbContext context, TrialPolicyService trialPolicy)
        {
            _context = context;
            _trialPolicy = trialPolicy;
        }

        [RequirePermission("Produits", "Voir")]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            var products = _context.Products
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            return View(products);
        }

        [HttpGet]
        [RequirePermission("Produits", "Créer")]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Produits", "Créer")]
        public IActionResult Create(Product product)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(product.Name))
            {
                ViewBag.Error = "Le nom du produit est obligatoire.";
                return View(product);
            }

            var productLimit = _trialPolicy.CanCreateProductAsync(HttpContext.RequestAborted).GetAwaiter().GetResult();
            if (!productLimit.Allowed)
            {
                ViewBag.Error = productLimit.Message;
                return View(product);
            }

            product.CreatedAt = DateTime.Now;

            _context.Products.Add(product);
            _context.SaveChanges();

            AuditService.Log(
                _context,
                HttpContext,
                "Produits",
                "Ajout",
                $"Produit ajouté : {product.Name}",
                entityId: product.Id.ToString()
            );

            return RedirectToAction(nameof(Index));
        }
    }
}
