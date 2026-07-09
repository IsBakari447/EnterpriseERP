using EnterpriseERP.Attributes;
using EnterpriseERP.Data;
using EnterpriseERP.Models;
using EnterpriseERP.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseERP.Controllers
{
    public class SuppliersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SuppliersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [RequirePermission("Fournisseurs", "Voir")]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            var suppliers = _context.Suppliers
                .OrderByDescending(s => s.CreatedAt)
                .ToList();

            return View(suppliers);
        }

        [HttpGet]
        [RequirePermission("Fournisseurs", "Créer")]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Fournisseurs", "Créer")]
        public IActionResult Create(Supplier supplier)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(supplier.Name))
            {
                ViewBag.Error = "Le nom du fournisseur est obligatoire.";
                return View(supplier);
            }

            supplier.CreatedAt = DateTime.Now;

            _context.Suppliers.Add(supplier);
            _context.SaveChanges();

            AuditService.Log(
                _context,
                HttpContext,
                "Fournisseurs",
                "Création",
                $"Fournisseur créé : {supplier.Name}",
                entityId: supplier.Id.ToString()
            );

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [RequirePermission("Fournisseurs", "Modifier")]
        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            var supplier = _context.Suppliers.FirstOrDefault(s => s.Id == id);

            if (supplier == null)
                return NotFound();

            return View(supplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Fournisseurs", "Modifier")]
        public IActionResult Edit(Supplier supplier)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(supplier.Name))
            {
                ViewBag.Error = "Le nom du fournisseur est obligatoire.";
                return View(supplier);
            }

            var existingSupplier = _context.Suppliers.FirstOrDefault(s => s.Id == supplier.Id);

            if (existingSupplier == null)
                return NotFound();

            string oldValue =
                $"Nom: {existingSupplier.Name}, Contact: {existingSupplier.ContactPerson}, Email: {existingSupplier.Email}, Téléphone: {existingSupplier.Phone}, Catégorie: {existingSupplier.Category}, Adresse: {existingSupplier.Address}";

            existingSupplier.Name = supplier.Name;
            existingSupplier.ContactPerson = supplier.ContactPerson;
            existingSupplier.Email = supplier.Email;
            existingSupplier.Phone = supplier.Phone;
            existingSupplier.Category = supplier.Category;
            existingSupplier.Address = supplier.Address;

            _context.SaveChanges();

            string newValue =
                $"Nom: {existingSupplier.Name}, Contact: {existingSupplier.ContactPerson}, Email: {existingSupplier.Email}, Téléphone: {existingSupplier.Phone}, Catégorie: {existingSupplier.Category}, Adresse: {existingSupplier.Address}";

            AuditService.Log(
                _context,
                HttpContext,
                "Fournisseurs",
                "Modification",
                $"Fournisseur modifié : {existingSupplier.Name}",
                oldValue: oldValue,
                newValue: newValue,
                entityId: existingSupplier.Id.ToString()
            );

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Fournisseurs", "Supprimer")]
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            var supplier = _context.Suppliers.FirstOrDefault(s => s.Id == id);

            if (supplier == null)
                return NotFound();

            string supplierName = supplier.Name;
            string entityId = supplier.Id.ToString();

            _context.Suppliers.Remove(supplier);
            _context.SaveChanges();

            AuditService.Log(
                _context,
                HttpContext,
                "Fournisseurs",
                "Suppression",
                $"Fournisseur supprimé : {supplierName}",
                severity: "Warning",
                entityId: entityId
            );

            return RedirectToAction(nameof(Index));
        }
    }
}