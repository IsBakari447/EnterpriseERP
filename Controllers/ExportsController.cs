using EnterpriseERP.Data;
using EnterpriseERP.Services;
using EnterpriseERP.Services.Export;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseERP.Controllers
{
    public class ExportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BrandingService _branding;

        public ExportsController(ApplicationDbContext context, BrandingService branding)
        {
            _context = context;
            _branding = branding;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        public async Task<IActionResult> Clients()
        {
            var brand = await _branding.GetBrandAsync();
            var data = await _context.Clients.OrderBy(x => x.FullName).ToListAsync();

            var file = ExcelExportService.ExportTable<object>(
                "Clients",
                new() { "Nom", "Entreprise", "Email", "Téléphone", "Adresse" },
                data.Select(x => new List<object?>
                {
                    x.FullName, x.CompanyName, x.Email, x.Phone, x.Address
                }).ToList(),
                brand,
                "Rapport Clients"
            );

            return Download(file, "clients.xlsx", "Clients");
        }

        public async Task<IActionResult> Employees()
        {
            var brand = await _branding.GetBrandAsync();
            var data = await _context.Employees.OrderBy(x => x.FullName).ToListAsync();

            var file = ExcelExportService.ExportTable<object>(
                "Employés",
                new() { "Nom", "Poste", "Email", "Téléphone", "Salaire" },
                data.Select(x => new List<object?>
                {
                    x.FullName, x.Position, x.Email, x.Phone, x.Salary
                }).ToList(),
                brand,
                "Rapport Employés"
            );

            return Download(file, "employes.xlsx", "Employés");
        }

        public async Task<IActionResult> Products()
        {
            var brand = await _branding.GetBrandAsync();
            var data = await _context.Products.OrderBy(x => x.Name).ToListAsync();

            var file = ExcelExportService.ExportTable<object>(
                "Produits",
                new() { "Produit", "Catégorie", "Prix achat", "Prix vente", "Quantité" },
                data.Select(x => new List<object?>
                {
                    x.Name, x.Category, x.PurchasePrice, x.SalePrice, x.Quantity
                }).ToList(),
                brand,
                "Rapport Produits"
            );

            return Download(file, "produits.xlsx", "Produits");
        }

        public async Task<IActionResult> Invoices()
        {
            var brand = await _branding.GetBrandAsync();
            var data = await _context.Invoices
                .Include(x => x.Client)
                .OrderByDescending(x => x.InvoiceDate)
                .ToListAsync();

            var file = ExcelExportService.ExportTable<object>(
                "Factures",
                new() { "N°", "Client", "Date", "Sous-total", "TVA", "Total", "Statut", "Méthode paiement" },
                data.Select(x => new List<object?>
                {
                    x.InvoiceNumber,
                    x.Client?.FullName,
                    x.InvoiceDate.ToString("dd/MM/yyyy"),
                    x.SubTotal,
                    x.VatAmount,
                    x.TotalAmount,
                    x.Status,
                    x.PaymentMethod
                }).ToList(),
                brand,
                "Rapport Factures"
            );

            return Download(file, "factures.xlsx", "Factures");
        }

        public async Task<IActionResult> Orders()
        {
            var brand = await _branding.GetBrandAsync();
            var data = await _context.Orders
                .Include(x => x.Client)
                .Include(x => x.Product)
                .OrderByDescending(x => x.OrderDate)
                .ToListAsync();

            var file = ExcelExportService.ExportTable<object>(
                "Commandes",
                new() { "N°", "Client", "Produit", "Quantité", "Prix unitaire", "Total", "Statut", "Date" },
                data.Select(x => new List<object?>
                {
                    x.Id,
                    x.Client?.FullName,
                    x.Product?.Name,
                    x.Quantity,
                    x.UnitPrice,
                    x.TotalAmount,
                    x.Status,
                    x.OrderDate.ToString("dd/MM/yyyy HH:mm")
                }).ToList(),
                brand,
                "Rapport Commandes"
            );

            return Download(file, "commandes.xlsx", "Commandes");
        }

        public async Task<IActionResult> Payments()
        {
            var brand = await _branding.GetBrandAsync();
            var data = await _context.Payments
                .Include(x => x.Invoice)
                .ThenInclude(i => i!.Client)
                .OrderByDescending(x => x.PaymentDate)
                .ToListAsync();

            var file = ExcelExportService.ExportTable<object>(
                "Paiements",
                new() { "Facture", "Client", "Montant", "Méthode", "Référence", "Date" },
                data.Select(x => new List<object?>
                {
                    x.InvoiceId,
                    x.Invoice?.Client?.FullName,
                    x.Amount,
                    x.Method,
                    x.Reference,
                    x.PaymentDate.ToString("dd/MM/yyyy HH:mm")
                }).ToList(),
                brand,
                "Rapport Paiements"
            );

            return Download(file, "paiements.xlsx", "Paiements");
        }

        public async Task<IActionResult> Suppliers()
        {
            var brand = await _branding.GetBrandAsync();
            var data = await _context.Suppliers.OrderBy(x => x.Name).ToListAsync();

            var file = ExcelExportService.ExportTable<object>(
                "Fournisseurs",
                new() { "Nom", "Contact", "Email", "Téléphone", "Catégorie", "Adresse" },
                data.Select(x => new List<object?>
                {
                    x.Name, x.ContactPerson, x.Email, x.Phone, x.Category, x.Address
                }).ToList(),
                brand,
                "Rapport Fournisseurs"
            );

            return Download(file, "fournisseurs.xlsx", "Fournisseurs");
        }

        public async Task<IActionResult> Stock()
        {
            var brand = await _branding.GetBrandAsync();
            var data = await _context.StockMovements
                .Include(x => x.Product)
                .OrderByDescending(x => x.Date)
                .ToListAsync();

            var file = ExcelExportService.ExportTable<object>(
                "Stock",
                new() { "Produit", "Type", "Quantité", "Date" },
                data.Select(x => new List<object?>
                {
                    x.Product?.Name,
                    x.Type,
                    x.Quantity,
                    x.Date.ToString("dd/MM/yyyy HH:mm")
                }).ToList(),
                brand,
                "Rapport Stock"
            );

            return Download(file, "stock.xlsx", "Stock");
        }

        public async Task<IActionResult> Presences()
        {
            var brand = await _branding.GetBrandAsync();
            var data = await _context.Presences
                .Include(x => x.Employee)
                .OrderByDescending(x => x.Date)
                .ToListAsync();

            var file = ExcelExportService.ExportTable<object>(
                "Présences",
                new() { "Employé", "Date", "Entrée", "Sortie", "Statut" },
                data.Select(x => new List<object?>
                {
                    x.Employee?.FullName,
                    x.Date.ToString("dd/MM/yyyy"),
                    x.CheckIn,
                    x.CheckOut,
                    x.CheckIn == null ? "Absent" : x.CheckOut == null ? "Présent" : "Sorti"
                }).ToList(),
                brand,
                "Rapport Présences"
            );

            return Download(file, "presences.xlsx", "Présences");
        }

        public async Task<IActionResult> Users()
        {
            var brand = await _branding.GetBrandAsync();
            var data = await _context.Users.OrderBy(x => x.FullName).ToListAsync();

            var file = ExcelExportService.ExportTable<object>(
                "Utilisateurs",
                new()
                {
                    "Nom", "Email", "Rôle", "Actif", "Approuvé",
                    "SuperAdmin", "Dernière connexion", "Connexions"
                },
                data.Select(x => new List<object?>
                {
                    x.FullName,
                    x.Email,
                    x.Role,
                    x.IsActive ? "Oui" : "Non",
                    x.IsApproved ? "Oui" : "Non",
                    x.IsSuperAdmin ? "Oui" : "Non",
                    x.LastConnection?.ToString("dd/MM/yyyy HH:mm"),
                    x.LoginCount
                }).ToList(),
                brand,
                "Rapport Utilisateurs"
            );

            return Download(file, "utilisateurs.xlsx", "Utilisateurs");
        }

        private FileContentResult Download(byte[] file, string filename, string module)
        {
            AuditService.Log(
                _context,
                HttpContext,
                "Export Excel",
                module,
                $"Export Excel professionnel du module {module} effectué : {filename}"
            );

            return File(
                file,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                filename
            );
        }
    }
}
