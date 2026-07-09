using EnterpriseERP.Attributes;
using EnterpriseERP.Data;
using EnterpriseERP.Models;
using EnterpriseERP.Services;
using EnterpriseERP.Services.Pdf;
using EnterpriseERP.Services.Trial;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Globalization;

namespace EnterpriseERP.Controllers
{
    public class InvoicesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly TrialPolicyService _trialPolicy;

        public InvoicesController(ApplicationDbContext context, TrialPolicyService trialPolicy)
        {
            _context = context;
            _trialPolicy = trialPolicy;
        }

        [RequirePermission("Factures", "Voir")]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            var invoices = _context.Invoices
                .Include(i => i.Client)
                .OrderByDescending(i => i.InvoiceDate)
                .ToList();

            return View(invoices);
        }

        [HttpGet]
        [RequirePermission("Factures", "Créer")]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            LoadCreateData();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Create")]
        [RequirePermission("Factures", "Créer")]
        public IActionResult CreatePost()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            var invoiceLimit = _trialPolicy.CanCreateInvoiceAsync(HttpContext.RequestAborted).GetAwaiter().GetResult();
            if (!invoiceLimit.Allowed)
            {
                ViewBag.Error = invoiceLimit.Message;
                LoadCreateData();
                return View("Create");
            }

            if (!int.TryParse(Request.Form["ClientId"], out int clientId) || clientId <= 0)
            {
                ViewBag.Error = "Veuillez sélectionner un client.";
                LoadCreateData();
                return View("Create");
            }

            string status = Request.Form["Status"].ToString();
            string paymentMethod = Request.Form["PaymentMethod"].ToString();
            string thankYouMessage = Request.Form["ThankYouMessage"].ToString();
            bool vatIncluded = Request.Form["VatIncluded"].ToString() == "true";

            decimal vatRate = ParseDecimal(Request.Form["VatRate"].ToString(), 20);

            var productIds = Request.Form["ProductId"].ToList();
            var productNames = Request.Form["ProductName"].ToList();
            var descriptions = Request.Form["Description"].ToList();
            var quantities = Request.Form["Quantity"].ToList();
            var unitPrices = Request.Form["UnitPrice"].ToList();
            var discounts = Request.Form["Discount"].ToList();

            var invoice = new Invoice
            {
                ClientId = clientId,
                InvoiceDate = DateTime.Now,
                InvoiceNumber = $"FAC-{DateTime.Now:yyyyMMddHHmmss}",
                Status = string.IsNullOrWhiteSpace(status) ? "Unpaid" : status,
                PaymentMethod = string.IsNullOrWhiteSpace(paymentMethod) ? "Cash" : paymentMethod,
                VatIncluded = vatIncluded,
                VatRate = vatRate,
                ThankYouMessage = string.IsNullOrWhiteSpace(thankYouMessage)
                    ? "Merci beaucoup pour votre confiance. Nous serons ravis de vous accueillir à nouveau la prochaine fois."
                    : thankYouMessage
            };

            decimal subTotal = 0;
            decimal vatAmount = 0;
            decimal totalTtc = 0;

            for (int i = 0; i < productNames.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(productNames[i]))
                    continue;

                int? productId = null;

                if (i < productIds.Count &&
                    int.TryParse(productIds[i], out int parsedProductId) &&
                    parsedProductId > 0)
                {
                    productId = parsedProductId;
                }

                decimal quantity = i < quantities.Count ? ParseDecimal(quantities[i], 1) : 1;
                decimal unitPrice = i < unitPrices.Count ? ParseDecimal(unitPrices[i], 0) : 0;
                decimal discount = i < discounts.Count ? ParseDecimal(discounts[i], 0) : 0;

                if (quantity <= 0 || unitPrice <= 0)
                    continue;

                decimal lineBase = quantity * unitPrice;
                decimal discountAmount = lineBase * discount / 100;
                decimal afterDiscount = lineBase - discountAmount;

                decimal lineHT;
                decimal lineVat;
                decimal lineTTC;

                if (vatIncluded)
                {
                    lineTTC = afterDiscount;
                    lineHT = afterDiscount / (1 + vatRate / 100);
                    lineVat = lineTTC - lineHT;
                }
                else
                {
                    lineHT = afterDiscount;
                    lineVat = lineHT * vatRate / 100;
                    lineTTC = lineHT + lineVat;
                }

                var item = new InvoiceItem
                {
                    ProductId = productId,
                    ProductName = productNames[i] ?? "",
                    Description = i < descriptions.Count ? descriptions[i] ?? "" : "",
                    Date = DateTime.Now,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    Discount = discount,
                    VatRate = vatRate,
                    VatIncluded = vatIncluded,
                    TotalHT = Math.Round(lineHT, 2),
                    VatAmount = Math.Round(lineVat, 2),
                    TotalTTC = Math.Round(lineTTC, 2)
                };

                invoice.Items.Add(item);

                subTotal += item.TotalHT;
                vatAmount += item.VatAmount;
                totalTtc += item.TotalTTC;
            }

            if (!invoice.Items.Any())
            {
                ViewBag.Error = "Veuillez ajouter au moins une ligne valide avec quantité et prix supérieur à 0.";
                LoadCreateData();
                return View("Create");
            }

            invoice.SubTotal = Math.Round(subTotal, 2);
            invoice.VatAmount = Math.Round(vatAmount, 2);
            invoice.TotalAmount = Math.Round(totalTtc, 2);

            _context.Invoices.Add(invoice);
            _context.SaveChanges();

            AuditService.Log(
                _context,
                HttpContext,
                "Factures",
                "Création",
                $"Création de la facture N° {invoice.InvoiceNumber}",
                entityId: invoice.Id.ToString()
            );

            return RedirectToAction(nameof(Details), new { id = invoice.Id });
        }

        [RequirePermission("Factures", "Voir")]
        public IActionResult Details(int id)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            var invoice = GetInvoice(id);

            if (invoice == null)
                return NotFound();

            return View(invoice);
        }

        [RequirePermission("Factures", "Voir")]
        public IActionResult Print(int id)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            var invoice = GetInvoice(id);

            if (invoice == null)
                return NotFound();

            AuditService.Log(
                _context,
                HttpContext,
                "Factures",
                "Aperçu",
                $"Aperçu de la facture N° {invoice.InvoiceNumber}",
                entityId: invoice.Id.ToString()
            );

            return View(invoice);
        }

        [RequirePermission("Factures", "Exporter")]
        public IActionResult DownloadPdf(int id)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            QuestPDF.Settings.License = LicenseType.Community;

            var invoice = GetInvoice(id);

            if (invoice == null)
                return NotFound();

            var document = new InvoicePdfDocument(invoice);
            byte[] pdf = document.GeneratePdf();

            AuditService.Log(
                _context,
                HttpContext,
                "Factures",
                "Export PDF",
                $"Facture PDF téléchargée : {invoice.InvoiceNumber}",
                entityId: invoice.Id.ToString()
            );

            return File(pdf, "application/pdf", $"{invoice.InvoiceNumber}.pdf");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Factures", "Supprimer")]
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            var invoice = _context.Invoices
                .Include(i => i.Items)
                .FirstOrDefault(i => i.Id == id);

            if (invoice == null)
                return NotFound();

            string invoiceNumber = invoice.InvoiceNumber;
            string entityId = invoice.Id.ToString();

            _context.Invoices.Remove(invoice);
            _context.SaveChanges();

            AuditService.Log(
                _context,
                HttpContext,
                "Factures",
                "Suppression",
                $"Facture supprimée : {invoiceNumber}",
                severity: "Warning",
                entityId: entityId
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

        private Invoice? GetInvoice(int id)
        {
            return _context.Invoices
                .Include(i => i.Client)
                .Include(i => i.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefault(i => i.Id == id);
        }

        private decimal ParseDecimal(string? value, decimal defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            value = value.Replace(",", ".");

            return decimal.TryParse(
                value,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out var result)
                ? result
                : defaultValue;
        }
    }
}
