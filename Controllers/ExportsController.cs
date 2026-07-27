using EnterpriseERP.Data;
using EnterpriseERP.Services;
using EnterpriseERP.Services.Export;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseERP.Controllers;

public class ExportsController : Controller
{
    private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

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

    public Task<IActionResult> Clients() =>
        ExportAsync(
            module: "Clients",
            fileName: "clients.xlsx",
            sheetName: "Clients",
            title: "Rapport Clients",
            headers: new() { "Nom", "Entreprise", "Email", "Téléphone", "Adresse" },
            rows: async () =>
            {
                var data = await _context.Clients.OrderBy(x => x.FullName).ToListAsync();
                return data.Select(x => new List<object?>
                {
                    x.FullName, x.CompanyName, x.Email, x.Phone, x.Address
                }).ToList();
            });

    public Task<IActionResult> Employees() =>
        ExportAsync(
            module: "Employés",
            fileName: "employes.xlsx",
            sheetName: "Employés",
            title: "Rapport Employés",
            headers: new() { "Nom", "Poste", "Email", "Téléphone", "Salaire" },
            rows: async () =>
            {
                var data = await _context.Employees.OrderBy(x => x.FullName).ToListAsync();
                return data.Select(x => new List<object?>
                {
                    x.FullName, x.Position, x.Email, x.Phone, x.Salary
                }).ToList();
            });

    public Task<IActionResult> Products() =>
        ExportAsync(
            module: "Produits",
            fileName: "produits.xlsx",
            sheetName: "Produits",
            title: "Rapport Produits",
            headers: new() { "Produit", "Catégorie", "Prix achat", "Prix vente", "Quantité" },
            rows: async () =>
            {
                var data = await _context.Products.OrderBy(x => x.Name).ToListAsync();
                return data.Select(x => new List<object?>
                {
                    x.Name, x.Category, x.PurchasePrice, x.SalePrice, x.Quantity
                }).ToList();
            });

    public Task<IActionResult> Invoices() =>
        ExportAsync(
            module: "Factures",
            fileName: "factures.xlsx",
            sheetName: "Factures",
            title: "Rapport Factures",
            headers: new() { "N°", "Client", "Date", "Sous-total", "TVA", "Total", "Statut", "Méthode paiement" },
            rows: async () =>
            {
                var data = await _context.Invoices
                    .Include(x => x.Client)
                    .OrderByDescending(x => x.InvoiceDate)
                    .ToListAsync();

                return data.Select(x => new List<object?>
                {
                    x.InvoiceNumber,
                    x.Client?.FullName,
                    x.InvoiceDate.ToString("dd/MM/yyyy"),
                    x.SubTotal,
                    x.VatAmount,
                    x.TotalAmount,
                    x.Status,
                    x.PaymentMethod
                }).ToList();
            });

    public Task<IActionResult> Orders() =>
        ExportAsync(
            module: "Commandes",
            fileName: "commandes.xlsx",
            sheetName: "Commandes",
            title: "Rapport Commandes",
            headers: new() { "N°", "Client", "Produit", "Quantité", "Prix unitaire", "Total", "Statut", "Date" },
            rows: async () =>
            {
                var data = await _context.Orders
                    .Include(x => x.Client)
                    .Include(x => x.Product)
                    .OrderByDescending(x => x.OrderDate)
                    .ToListAsync();

                return data.Select(x => new List<object?>
                {
                    x.Id,
                    x.Client?.FullName,
                    x.Product?.Name,
                    x.Quantity,
                    x.UnitPrice,
                    x.TotalAmount,
                    x.Status,
                    x.OrderDate.ToString("dd/MM/yyyy HH:mm")
                }).ToList();
            });

    public Task<IActionResult> Payments() =>
        ExportAsync(
            module: "Paiements",
            fileName: "paiements.xlsx",
            sheetName: "Paiements",
            title: "Rapport Paiements",
            headers: new() { "Facture", "Client", "Montant", "Méthode", "Référence", "Date" },
            rows: async () =>
            {
                var data = await _context.Payments
                    .Include(x => x.Invoice)
                    .ThenInclude(i => i!.Client)
                    .OrderByDescending(x => x.PaymentDate)
                    .ToListAsync();

                return data.Select(x => new List<object?>
                {
                    x.InvoiceId,
                    x.Invoice?.Client?.FullName,
                    x.Amount,
                    x.Method,
                    x.Reference,
                    x.PaymentDate.ToString("dd/MM/yyyy HH:mm")
                }).ToList();
            });

    public Task<IActionResult> Suppliers() =>
        ExportAsync(
            module: "Fournisseurs",
            fileName: "fournisseurs.xlsx",
            sheetName: "Fournisseurs",
            title: "Rapport Fournisseurs",
            headers: new() { "Nom", "Contact", "Email", "Téléphone", "Catégorie", "Adresse" },
            rows: async () =>
            {
                var data = await _context.Suppliers.OrderBy(x => x.Name).ToListAsync();
                return data.Select(x => new List<object?>
                {
                    x.Name, x.ContactPerson, x.Email, x.Phone, x.Category, x.Address
                }).ToList();
            });

    public Task<IActionResult> Stock() =>
        ExportAsync(
            module: "Stock",
            fileName: "stock.xlsx",
            sheetName: "Stock",
            title: "Rapport Stock",
            headers: new() { "Produit", "Type", "Quantité", "Date" },
            rows: async () =>
            {
                var data = await _context.StockMovements
                    .Include(x => x.Product)
                    .OrderByDescending(x => x.Date)
                    .ToListAsync();

                return data.Select(x => new List<object?>
                {
                    x.Product?.Name,
                    x.Type,
                    x.Quantity,
                    x.Date.ToString("dd/MM/yyyy HH:mm")
                }).ToList();
            });

    public Task<IActionResult> Presences() =>
        ExportAsync(
            module: "Présences",
            fileName: "presences.xlsx",
            sheetName: "Présences",
            title: "Rapport Présences",
            headers: new() { "Employé", "Date", "Entrée", "Sortie", "Statut" },
            rows: async () =>
            {
                var data = await _context.Presences
                    .Include(x => x.Employee)
                    .OrderByDescending(x => x.Date)
                    .ToListAsync();

                return data.Select(x => new List<object?>
                {
                    x.Employee?.FullName,
                    x.Date.ToString("dd/MM/yyyy"),
                    x.CheckIn,
                    x.CheckOut,
                    x.CheckIn == null ? "Absent" : x.CheckOut == null ? "Présent" : "Sorti"
                }).ToList();
            });

    public Task<IActionResult> Users() =>
        ExportAsync(
            module: "Utilisateurs",
            fileName: "utilisateurs.xlsx",
            sheetName: "Utilisateurs",
            title: "Rapport Utilisateurs",
            headers: new()
            {
                "Nom", "Email", "Rôle", "Actif", "Approuvé",
                "SuperAdmin", "Dernière connexion", "Connexions"
            },
            rows: async () =>
            {
                var data = await _context.Users.OrderBy(x => x.FullName).ToListAsync();
                return data.Select(x => new List<object?>
                {
                    x.FullName,
                    x.Email,
                    x.Role,
                    x.IsActive ? "Oui" : "Non",
                    x.IsApproved ? "Oui" : "Non",
                    x.IsSuperAdmin ? "Oui" : "Non",
                    x.LastConnection?.ToString("dd/MM/yyyy HH:mm"),
                    x.LoginCount
                }).ToList();
            });

    private async Task<IActionResult> ExportAsync(
        string module,
        string fileName,
        string sheetName,
        string title,
        List<string> headers,
        Func<Task<List<List<object?>>>> rows)
    {
        var brand = await _branding.GetBrandAsync();
        var file = ExcelExportService.ExportTable<object>(
            sheetName,
            headers,
            await rows(),
            brand,
            title);

        AuditService.Log(
            _context,
            HttpContext,
            "Export Excel",
            module,
            $"Export Excel professionnel du module {module} effectué : {fileName}");

        return File(file, ExcelContentType, fileName);
    }
}
